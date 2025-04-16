using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Application.DTOs;
using TodoApi.Domain.Entities;
using TodoApi.Domain.Repositories;

namespace TodoApi.Application.Services
{
    /// <summary>
    /// Todoアイテムに関するアプリケーションサービス
    /// アプリケーションサービスはユースケースを実現するためのオーケストレーターです
    /// </summary>
    public class TodoService
    {
        private readonly ITodoRepository _todoRepository;

        /// <summary>
        /// コンストラクタ - 依存性注入でリポジトリを受け取る
        /// </summary>
        /// <param name="todoRepository">Todoリポジトリ</param>
        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
        }

        /// <summary>
        /// すべてのTodoアイテムを取得
        /// </summary>
        /// <returns>TodoDtoのリスト</returns>
        public async Task<List<TodoDto>> GetAllTodosAsync()
        {
            var todos = await _todoRepository.GetAllAsync();

            // エンティティからDTOへの変換
            return todos.Select(todo => new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            }).ToList();
        }

        /// <summary>
        /// IDに基づいてTodoアイテムを取得
        /// </summary>
        /// <param name="id">取得するTodoのID</param>
        /// <returns>TodoDto、またはnull</returns>
        public async Task<TodoDto> GetTodoByIdAsync(Guid id)
        {
            var todo = await _todoRepository.GetByIdAsync(id);
            if (todo == null)
                return null;

            // エンティティからDTOへの変換
            return new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            };
        }

        /// <summary>
        /// 新しいTodoアイテムを作成
        /// </summary>
        /// <param name="createTodoDto">作成情報</param>
        /// <returns>作成されたTodoのID</returns>
        public async Task<Guid> CreateTodoAsync(CreateTodoDto createTodoDto)
        {
            // DTOの検証
            if (createTodoDto == null)
                throw new ArgumentNullException(nameof(createTodoDto));

            if (string.IsNullOrWhiteSpace(createTodoDto.Title))
                throw new ArgumentException("Todoのタイトルは必須です", nameof(createTodoDto.Title));

            // ドメインエンティティの作成
            var todo = Todo.Create(createTodoDto.Title);

            // リポジトリに保存
            return await _todoRepository.AddAsync(todo);
        }

        /// <summary>
        /// 既存のTodoアイテムを更新
        /// </summary>
        /// <param name="id">更新するTodoのID</param>
        /// <param name="updateTodoDto">更新情報</param>
        /// <returns>更新が成功したかどうか</returns>
        public async Task<bool> UpdateTodoAsync(Guid id, UpdateTodoDto updateTodoDto)
        {
            // DTOの検証
            if (updateTodoDto == null)
                throw new ArgumentNullException(nameof(updateTodoDto));

            // 既存のTodoを取得
            var todo = await _todoRepository.GetByIdAsync(id);
            if (todo == null)
                return false;

            // 更新すべき属性がある場合は更新
            if (!string.IsNullOrWhiteSpace(updateTodoDto.Title))
            {
                todo.ChangeTitle(updateTodoDto.Title);
            }

            if (updateTodoDto.IsCompleted.HasValue)
            {
                if (updateTodoDto.IsCompleted.Value)
                    todo.MarkAsCompleted();
                else
                    todo.MarkAsIncomplete();
            }

            // リポジトリに保存
            return await _todoRepository.UpdateAsync(todo);
        }

        /// <summary>
        /// Todoアイテムを削除
        /// </summary>
        /// <param name="id">削除するTodoのID</param>
        /// <returns>削除が成功したかどうか</returns>
        public async Task<bool> DeleteTodoAsync(Guid id)
        {
            return await _todoRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Todoアイテムの完了状態を切り替え
        /// </summary>
        /// <param name="id">対象のTodoのID</param>
        /// <returns>更新が成功したかどうか</returns>
        public async Task<bool> ToggleTodoCompletionAsync(Guid id)
        {
            var todo = await _todoRepository.GetByIdAsync(id);
            if (todo == null)
                return false;

            todo.ToggleCompletion();
            return await _todoRepository.UpdateAsync(todo);
        }
    }
}