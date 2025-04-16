using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.DTOs;
using TodoApi.Application.Services;

namespace TodoApi.Presentation.Controllers
{
    /// <summary>
    /// Todo項目を管理するAPIコントローラ
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly TodoService _todoService;

        /// <summary>
        /// コンストラクタ - 依存性の注入
        /// </summary>
        /// <param name="todoService">Todoサービス</param>
        public TodosController(TodoService todoService)
        {
            _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
        }

        /// <summary>
        /// すべてのTodoアイテムを取得
        /// </summary>
        /// <returns>Todoのリスト</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetAll()
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        /// <summary>
        /// 指定したIDのTodoアイテムを取得
        /// </summary>
        /// <param name="id">TodoのID</param>
        /// <returns>Todoアイテム</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDto>> GetById(Guid id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);

            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        /// <summary>
        /// 新しいTodoアイテムを作成
        /// </summary>
        /// <param name="createTodoDto">作成するTodoの情報</param>
        /// <returns>作成されたTodo</returns>
        [HttpPost]
        public async Task<ActionResult<TodoDto>> Create(CreateTodoDto createTodoDto)
        {
            try
            {
                var todoId = await _todoService.CreateTodoAsync(createTodoDto);
                var createdTodo = await _todoService.GetTodoByIdAsync(todoId);

                return CreatedAtAction(nameof(GetById), new { id = todoId }, createdTodo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 既存のTodoアイテムを更新
        /// </summary>
        /// <param name="id">更新するTodoのID</param>
        /// <param name="updateTodoDto">更新情報</param>
        /// <returns>更新結果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTodoDto updateTodoDto)
        {
            try
            {
                var result = await _todoService.UpdateTodoAsync(id, updateTodoDto);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Todoアイテムを削除
        /// </summary>
        /// <param name="id">削除するTodoのID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _todoService.DeleteTodoAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Todoの完了状態を切り替え
        /// </summary>
        /// <param name="id">対象のTodoのID</param>
        /// <returns>更新結果</returns>
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleCompletion(Guid id)
        {
            var result = await _todoService.ToggleTodoCompletionAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}