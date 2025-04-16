using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Postgrest.Responses;
using Supabase;
using TodoApi.Domain.Entities;
using TodoApi.Domain.Repositories;

namespace TodoApi.Infrastructure.Repositories
{
    /// <summary>
    /// Supabaseを使用したTodoリポジトリの実装
    /// </summary>
    public class SupabaseTodoRepository : ITodoRepository
    {
        private readonly Client _supabaseClient;

        /// <summary>
        /// コンストラクタ - Supabaseクライアントを受け取る
        /// </summary>
        /// <param name="supabaseClient">Supabaseクライアント</param>
        public SupabaseTodoRepository(Client supabaseClient)
        {
            _supabaseClient = supabaseClient ?? throw new ArgumentNullException(nameof(supabaseClient));
        }

        /// <summary>
        /// すべてのTodoを取得
        /// </summary>
        /// <returns>Todoのリスト</returns>
        public async Task<List<Todo>> GetAllAsync()
        {
            try
            {
                // Supabaseからtodoテーブルの全データを取得
                // 作成日の降順（新しい順）で取得
                var response = await _supabaseClient
                    .From<TodoRecord>()
                    .Order("created_at", Postgrest.Constants.Ordering.Descending)
                    .Get();

                // レコードをドメインエンティティに変換
                return response.Models.Select(MapToDomainEntity).ToList();
            }
            catch (Exception ex)
            {
                // エラーログを出力するなどの処理をここに追加できます
                Console.WriteLine($"GetAllAsync エラー: {ex.Message}");
                return new List<Todo>();
            }
        }

        /// <summary>
        /// IDによるTodoの取得
        /// </summary>
        /// <param name="id">TodoのID</param>
        /// <returns>見つかったTodo、または null</returns>
        public async Task<Todo> GetByIdAsync(Guid id)
        {
            try
            {
                Console.WriteLine($"GetByIdAsync: ID={id}の取得を試行");

                // Single()ではなくGet()を使用し、フィルタリングする
                var response = await _supabaseClient
                    .From<TodoRecord>()
                    .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                // レスポンスをログに出力
                Console.WriteLine($"GetByIdAsync: レスポンス取得 - アイテム数: {response?.Models?.Count ?? 0}");

                // レスポンスからアイテムを取得
                var todoRecord = response?.Models?.FirstOrDefault();

                if (todoRecord == null)
                {
                    Console.WriteLine("GetByIdAsync: アイテムが見つかりませんでした");
                    return null;
                }

                Console.WriteLine($"GetByIdAsync: アイテム取得 - Title: {todoRecord.Title}");
                return MapToDomainEntity(todoRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetByIdAsync エラー: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 新しいTodoを追加
        /// </summary>
        /// <param name="todo">追加するTodo</param>
        /// <returns>追加されたTodoのID</returns>
        public async Task<Guid> AddAsync(Todo todo)
        {
            try
            {
                // ドメインエンティティからレコードに変換
                var todoRecord = new TodoRecord
                {

                    // Id = todo.Id.ToString(),  // // C#側で生成したIDを使わず、Supabase側で生成させる為、この行を削除またはコメントアウト
                    Title = todo.Title,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt,
                    UserId = todo.UserId?.ToString()
                };

                // Supabaseに保存
                var response = await _supabaseClient
                    .From<TodoRecord>()
                    .Insert(todoRecord);

                // 応答から生成されたIDを取得
                if (response?.Models?.FirstOrDefault() is TodoRecord insertedRecord &&
                    !string.IsNullOrEmpty(insertedRecord.Id))
                {
                    return Guid.Parse(insertedRecord.Id);
                }

                // 通常ここには到達しないはず
                throw new Exception("Todoの挿入に成功しましたが、IDが返されませんでした");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddAsync エラー: {ex.Message}");
                throw; // エラーを上位層に伝播
            }
        }

        /// <summary>
        /// 既存のTodoを更新
        /// </summary>
        /// <param name="todo">更新するTodo</param>
        /// <returns>更新が成功したかどうか</returns>
        public async Task<bool> UpdateAsync(Todo todo)
        {
            try
            {
                // ドメインエンティティからレコードに変換
                var todoRecord = new TodoRecord
                {
                    Id = todo.Id.ToString(),
                    Title = todo.Title,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt,
                    UserId = todo.UserId?.ToString()
                };

                // レコードを更新
                await _supabaseClient
                    .From<TodoRecord>()
                    .Where(t => t.Id == todo.Id.ToString())
                    .Update(todoRecord);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateAsync エラー: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TodoをIDで削除
        /// </summary>
        /// <param name="id">削除するTodoのID</param>
        /// <returns>削除が成功したかどうか</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                // IDでTodoを削除
                await _supabaseClient
                    .From<TodoRecord>()
                    .Where(t => t.Id == id.ToString())
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteAsync エラー: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// データベースレコードからドメインエンティティへの変換
        /// </summary>
        /// <param name="record">Supabaseから取得したレコード</param>
        /// <returns>ドメインエンティティ</returns>
        private static Todo MapToDomainEntity(TodoRecord record)
        {
            // プライベートコンストラクタを使用するためにリフレクションを使うか、
            // ファクトリメソッドを通してインスタンスを作成して必要なプロパティを設定
            var todo = Todo.Create(record.Title);

            // リフレクションなどを使って非公開プロパティを設定
            // ここでは簡略化のため、プロパティの内部状態を直接操作（実際のプロジェクトでは適切に設計）
            var type = todo.GetType();

            type.GetProperty("Id").SetValue(todo, Guid.Parse(record.Id));
            type.GetProperty("IsCompleted").SetValue(todo, record.IsCompleted);
            type.GetProperty("CreatedAt").SetValue(todo, record.CreatedAt);

            if (!string.IsNullOrEmpty(record.UserId))
            {
                type.GetProperty("UserId").SetValue(todo, Guid.Parse(record.UserId));
            }

            return todo;
        }
    }

    /// <summary>
    /// Supabaseデータベースとのマッピングのための内部クラス
    /// </summary>
    [Postgrest.Attributes.Table("todos")]
    public class TodoRecord : Postgrest.Models.BaseModel
    {
        [Postgrest.Attributes.PrimaryKey("id")]
        public string Id { get; set; }

        [Postgrest.Attributes.Column("title")]
        public string Title { get; set; }

        [Postgrest.Attributes.Column("is_completed")]
        public bool IsCompleted { get; set; }

        [Postgrest.Attributes.Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Postgrest.Attributes.Column("user_id")]
        public string UserId { get; set; }
    }
}