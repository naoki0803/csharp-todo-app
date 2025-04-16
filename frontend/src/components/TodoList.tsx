'use client';

import { useState, useEffect } from 'react';
import TodoItem from './TodoItem';
import TodoForm from './TodoForm';
import { Todo, todoApi } from '@/lib/api-client';
import { Toaster } from './ui/sonner';
import { toast } from 'sonner';
import { Card, CardHeader, CardTitle } from './ui/card';

/**
 * TodoListコンポーネント
 * アプリケーションのメインコンポーネント
 * 
 * @returns TodoListコンポーネント
 */
export default function TodoList() {
  // Todo一覧の状態
  const [todos, setTodos] = useState<Todo[]>([]);
  // ロード中フラグ
  const [loading, setLoading] = useState(true);
  // エラー状態
  const [error, setError] = useState<string | null>(null);
  
  /**
   * コンポーネントのマウント時にTodo一覧を取得
   */
  useEffect(() => {
    // Todoアイテムを取得する非同期関数
    const fetchTodos = async () => {
      try {
        setLoading(true);
        const data = await todoApi.getAll();
        setTodos(data);
        setError(null);
      } catch (err) {
        console.error('Todoの取得に失敗しました', err);
        setError('Todoの取得に失敗しました。');
        toast.error('Todoの取得に失敗しました。');
      } finally {
        setLoading(false);
      }
    };
    
    fetchTodos();
  }, []);
  
  /**
   * 新しいTodoを追加
   */
  const handleAddTodo = async (title: string) => {
    try {
      const newTodo = await todoApi.create({ title });
      console.log("🚀 ~ handleAddTodo ~ newTodo:", newTodo)
      setTodos(prevTodos => [newTodo, ...prevTodos]);
      toast.success('Todoを追加しました');
    } catch (err) {
      console.error('Todoの追加に失敗しました', err);
      toast.error('Todoの追加に失敗しました');
    }
  };
  
  /**
   * Todoを削除
   */
  const handleDeleteTodo = async (id: string) => {
    try {
      await todoApi.delete(id);
      setTodos(prevTodos => prevTodos.filter(todo => todo.id !== id));
      toast.success('Todoを削除しました');
    } catch (err) {
      console.error('Todoの削除に失敗しました', err);
      toast.error('Todoの削除に失敗しました');
    }
  };
  
  /**
   * Todoの完了状態を切り替え
   */
  const handleToggleTodo = async (id: string) => {
    try {
      await todoApi.toggleCompletion(id);
      
      // 状態を更新
      setTodos(prevTodos =>
        prevTodos.map(todo =>
          todo.id === id
            ? { ...todo, isCompleted: !todo.isCompleted }
            : todo
        )
      );
      
      // 成功メッセージを表示
      const toggledTodo = todos.find(todo => todo.id === id);
      const status = toggledTodo && !toggledTodo.isCompleted
        ? '完了'
        : '未完了';
      toast.success(`Todoを${status}にしました`);
    } catch (err) {
      console.error('Todoの状態変更に失敗しました', err);
      toast.error('Todoの状態変更に失敗しました');
    }
  };
  
  /**
   * Todoのタイトルを更新
   */
  const handleUpdateTodo = async (id: string, title: string) => {
    try {
      await todoApi.update(id, { title });
      
      // 状態を更新
      setTodos(prevTodos =>
        prevTodos.map(todo =>
          todo.id === id
            ? { ...todo, title }
            : todo
        )
      );
      
      toast.success('Todoを更新しました');
    } catch (err) {
      console.error('Todoの更新に失敗しました', err);
      toast.error('Todoの更新に失敗しました');
    }
  };

  return (
    <div className="container max-w-2xl mx-auto p-4">
      <Toaster />
      
      <Card className="mb-6">
        <CardHeader>
          <CardTitle className="text-2xl text-center">
            Todoリスト
          </CardTitle>
        </CardHeader>
      </Card>
      
      {/* Todo追加フォーム */}
      <TodoForm onAdd={handleAddTodo} />
      
      {/* ローディング中表示 */}
      {loading && (
        <div className="text-center p-4">
          <p>読み込み中...</p>
        </div>
      )}
      
      {/* エラー表示 */}
      {error && (
        <div className="text-center p-4 text-red-500">
          <p>{error}</p>
        </div>
      )}
      
      {/* Todo一覧 */}
      {!loading && !error && todos.length === 0 && (
        <div className="text-center p-4 text-gray-500">
          <p>Todoがありません。新しいTodoを追加してください。</p>
        </div>
      )}
      
      {/* Todoアイテム一覧 */}
      {todos.map(todo => (
        <TodoItem
          key={todo.id}
          todo={todo}
          onDelete={handleDeleteTodo}
          onToggle={handleToggleTodo}
          onUpdate={handleUpdateTodo}
        />
      ))}
    </div>
  );
} 