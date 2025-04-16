using System;

namespace TodoApi.Application.DTOs
{
    /// <summary>
    /// TodoアイテムのData Transfer Object（DTO）
    /// DTO（データ転送オブジェクト）はレイヤー間でデータを転送するための単純なオブジェクトです
    /// </summary>
    public class TodoDto
    {
        /// <summary>
        /// TodoアイテムのID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Todoのタイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 新しいTodoアイテムを作成するためのDTO
    /// </summary>
    public class CreateTodoDto
    {
        /// <summary>
        /// 新しいTodoのタイトル
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 既存のTodoアイテムを更新するためのDTO
    /// </summary>
    public class UpdateTodoDto
    {
        /// <summary>
        /// 更新するTodoのタイトル（オプション）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 更新する完了状態（オプション）
        /// </summary>
        public bool? IsCompleted { get; set; }
    }
}