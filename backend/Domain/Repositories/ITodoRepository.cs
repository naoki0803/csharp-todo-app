using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Domain.Entities;

namespace TodoApi.Domain.Repositories
{
    /// <summary>
    /// Todoエンティティのリポジトリインターフェース
    /// DDD（ドメイン駆動設計）では、リポジトリはデータの永続化と取得を抽象化します
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// すべてのTodoアイテムを取得
        /// </summary>
        /// <returns>Todoアイテムのリスト</returns>
        Task<List<Todo>> GetAllAsync();

        /// <summary>
        /// IDに基づいてTodoアイテムを取得
        /// </summary>
        /// <param name="id">取得するTodoのID</param>
        /// <returns>見つかったTodoアイテム、または null</returns>
        Task<Todo> GetByIdAsync(Guid id);

        /// <summary>
        /// 新しいTodoアイテムを追加
        /// </summary>
        /// <param name="todo">追加するTodoエンティティ</param>
        /// <returns>追加されたTodoのID</returns>
        Task<Guid> AddAsync(Todo todo);

        /// <summary>
        /// 既存のTodoアイテムを更新
        /// </summary>
        /// <param name="todo">更新するTodoエンティティ</param>
        /// <returns>更新が成功したかどうか</returns>
        Task<bool> UpdateAsync(Todo todo);

        /// <summary>
        /// Todoアイテムを削除
        /// </summary>
        /// <param name="id">削除するTodoのID</param>
        /// <returns>削除が成功したかどうか</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}