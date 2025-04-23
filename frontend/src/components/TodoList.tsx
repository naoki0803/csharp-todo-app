'use client';

import { useTodos } from '@/hooks/useTodos';
import TodoItem from './TodoItem';
import TodoForm from './TodoForm';
import { Toaster } from './ui/sonner';
import { Card, CardHeader, CardTitle } from './ui/card';
import { Todo } from '@/lib/api-client';

/**
 * TodoListコンポーネント
 * アプリケーションのメインコンポーネント
 * 
 * @returns TodoListコンポーネント
 */
export default function TodoList() {
  const {
    todos,
    loading,
    error,
    addTodo,
    deleteTodo,
    toggleTodo,
    updateTodo
  } = useTodos();

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
      <TodoForm onAdd={addTodo} />
      
      {/* ローディング中表示 */}
      {loading && <LoadingView />}
      
      {/* エラー表示 */}
      {error && <ErrorView error={error} />}
      
      {/* Todo一覧 */}
      {!loading && !error && todos.length === 0 && <EmptyView />}
      
      {/* Todoアイテム一覧 */}
      <TodoItemList
        todos={todos}
        onDelete={deleteTodo}
        onToggle={toggleTodo}
        onUpdate={updateTodo}
      />
    </div>
  );
}

const LoadingView = () => (
  <div className="text-center p-4">
    <p>読み込み中...</p>
  </div>
);

const ErrorView = ({ error }: { error: string }) => (
  <div className="text-center p-4 text-red-500">
    <p>{error}</p>
  </div>
);

const EmptyView = () => (
  <div className="text-center p-4 text-gray-500">
    <p>Todoがありません。新しいTodoを追加してください。</p>
  </div>
);

const TodoItemList = ({
  todos,
  onDelete,
  onToggle,
  onUpdate
}: {
  todos: Todo[];
  onDelete: (id: string) => Promise<void>;
  onToggle: (id: string) => Promise<void>;
  onUpdate: (id: string, title: string) => Promise<void>;
}) => (
  <>
    {todos.map(todo => (
      <TodoItem
        key={todo.id}
        todo={todo}
        onDelete={onDelete}
        onToggle={onToggle}
        onUpdate={onUpdate}
      />
    ))}
  </>
); 