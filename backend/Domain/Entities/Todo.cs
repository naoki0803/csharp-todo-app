using System;

namespace TodoApi.Domain.Entities
{
    /// <summary>
    /// Todoアイテムを表すエンティティクラス
    /// DDD（ドメイン駆動設計）では、エンティティはビジネスロジックとデータを持つオブジェクトです
    /// </summary>
    public class Todo
    {
        /// <summary>
        /// TodoアイテムのID（一意の識別子）
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Todoのタイトル
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// ユーザーID（拡張機能のために用意）
        /// </summary>
        public Guid? UserId { get; private set; }

        // プライベートコンストラクタ - ファクトリメソッド経由での作成を強制
        private Todo() { }

        /// <summary>
        /// 新しいTodoアイテムを作成するファクトリメソッド
        /// </summary>
        /// <param name="title">Todoのタイトル</param>
        /// <param name="userId">ユーザーID（オプション）</param>
        /// <returns>作成されたTodoエンティティ</returns>
        public static Todo Create(string title, Guid? userId = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Todoのタイトルは必須です", nameof(title));

            return new Todo
            {
                Id = Guid.NewGuid(),
                Title = title,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };
        }

        /// <summary>
        /// Todoのタイトルを変更
        /// </summary>
        /// <param name="newTitle">新しいタイトル</param>
        public void ChangeTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Todoのタイトルは必須です", nameof(newTitle));

            Title = newTitle;
        }

        /// <summary>
        /// Todoを完了状態に変更
        /// </summary>
        public void MarkAsCompleted()
        {
            IsCompleted = true;
        }

        /// <summary>
        /// Todoを未完了状態に変更
        /// </summary>
        public void MarkAsIncomplete()
        {
            IsCompleted = false;
        }

        /// <summary>
        /// 完了ステータスを切り替え
        /// </summary>
        public void ToggleCompletion()
        {
            IsCompleted = !IsCompleted;
        }
    }
}