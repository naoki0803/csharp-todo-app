/**
 * TodoアプリのAPIクライアント
 * バックエンドとの通信を担当するモジュール
 */

// バックエンドAPIのベースURL
// 開発環境では、.envファイルなどで環境変数として管理するとよい
const API_BASE_URL = 'http://localhost:5078/api';

/**
 * TodoアイテムのインターフェイスA
 * バックエンドのTodoDtoと一致させる
 */
export interface Todo {
  id: string;       // TodoのID
  title: string;    // Todoのタイトル
  isCompleted: boolean;  // 完了フラグ
  createdAt: string;     // 作成日時（文字列形式）
}

/**
 * 新しいTodoアイテムの作成に使用するインターフェイス
 * バックエンドのCreateTodoDtoと一致させる
 */
export interface CreateTodo {
  title: string;    // 新しいTodoのタイトル
}

/**
 * 既存のTodoアイテム更新に使用するインターフェイス
 * バックエンドのUpdateTodoDtoと一致させる
 */
export interface UpdateTodo {
  title?: string;     // 更新するタイトル（オプション）
  isCompleted?: boolean;  // 更新する完了状態（オプション）
}

/**
 * APIリクエスト実行時のエラーハンドリング用のヘルパー関数
 */
async function handleResponse<T>(response: Response): Promise<T> {
  // console.log('handleResponse', response);
  if (!response.ok) {
    // レスポンスのステータスコードが200番台でない場合はエラーとする
    const errorText = await response.text();
    throw new Error(errorText || `HTTPエラー ${response.status}`);
  }
  
  // 空の場合はnullを返す（DELETEリクエストなど）
  if (response.status === 204) {
    return null as unknown as T;
  }
        /// <summary>
  
  // JSONレスポンスをパース
  return await response.json() as T;
}

/**
 * TodoのAPIクライアント
 * バックエンドとの通信を行うための関数を提供
 */
export const todoApi = {
  /**
   * すべてのTodoを取得
   */
  async getAll(): Promise<Todo[]> {
    const response = await fetch(`${API_BASE_URL}/todos`);
    return handleResponse<Todo[]>(response);
  },
  
  /**
   * IDを指定してTodoを1件取得
   */
  async getById(id: string): Promise<Todo> {
    const response = await fetch(`${API_BASE_URL}/todos/${id}`);
    return handleResponse<Todo>(response);
  },
  
  /**
   * 新しいTodoを作成
   */
  async create(todo: CreateTodo): Promise<Todo> {
    const response = await fetch(`${API_BASE_URL}/todos`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(todo),
    });
    return handleResponse<Todo>(response);
  },
  
  /**
   * 既存のTodoを更新
   */
  async update(id: string, todo: UpdateTodo): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(todo),
    });
    return handleResponse<void>(response);
  },
  
  /**
   * Todoを削除
   */
  async delete(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
      method: 'DELETE',
    });
    return handleResponse<void>(response);
  },
  
  /**
   * Todoの完了状態を切り替え
   */
  async toggleCompletion(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/todos/${id}/toggle`, {
      method: 'PATCH',
    });
    return handleResponse<void>(response);
  },
}; 