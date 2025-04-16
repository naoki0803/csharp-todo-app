'use client';

import { useState } from 'react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Card, CardContent } from './ui/card';

/**
 * TodoForm コンポーネントのプロパティ
 */
interface TodoFormProps {
  onAdd: (title: string) => void;  // Todo追加時に呼び出されるコールバック
}

/**
 * 新しいTodoを追加するためのフォームコンポーネント
 * 
 * @param props - コンポーネントのプロパティ
 * @returns Todoフォームコンポーネント
 */
export default function TodoForm({ onAdd }: TodoFormProps) {
  // 入力中のタイトル
  const [title, setTitle] = useState('');
  // 送信中フラグ（連続送信防止）
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  /**
   * フォーム送信時の処理
   */
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // タイトルが空の場合は何もしない
    if (!title.trim()) return;
    
    setIsSubmitting(true);
    
    // 親コンポーネントが提供するコールバック関数を呼び出す
    onAdd(title);
    
    // フォームをリセット
    setTitle('');
    setIsSubmitting(false);
  };

  return (
    <Card className="mb-6">
      <CardContent className="p-4">
        <form onSubmit={handleSubmit} className="flex gap-2">
          {/* タイトル入力欄 */}
          <Input
            type="text"
            placeholder="新しいタスクを入力..."
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="flex-1"
            disabled={isSubmitting}
          />
          
          {/* 追加ボタン */}
          <Button type="submit" disabled={isSubmitting || !title.trim()}>
            追加
          </Button>
        </form>
      </CardContent>
    </Card>
  );
} 