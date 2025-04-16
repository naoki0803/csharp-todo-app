'use client';

import { useState } from 'react';
import { Checkbox } from './ui/checkbox';
import { Button } from './ui/button';
import { Todo } from '@/lib/api-client';
import { Card, CardContent } from './ui/card';
import { formatDistanceToNow } from 'date-fns';
import { ja } from 'date-fns/locale';

/**
 * TodoItem コンポーネントのプロパティ
 */
interface TodoItemProps {
  todo: Todo;                 // Todoアイテムのデータ
  onDelete: (id: string) => void;      // 削除時のコールバック
  onToggle: (id: string) => void;      // 完了状態切替時のコールバック
  onUpdate: (id: string, title: string) => void;  // 更新時のコールバック
}

/**
 * 個々のTodoアイテムを表示するコンポーネント
 * 
 * @param props - コンポーネントのプロパティ
 * @returns Todoアイテムコンポーネント
 */
export default function TodoItem({ todo, onDelete, onToggle, onUpdate }: TodoItemProps) {
  // 編集モードの状態管理
  const [isEditing, setIsEditing] = useState(false);
  // 編集中のタイトル
  const [editTitle, setEditTitle] = useState(todo.title);
  
  /**
   * 編集完了時の処理
   */
  const handleSubmit = () => {
    if (editTitle.trim()) {
      onUpdate(todo.id, editTitle);
      setIsEditing(false);
    }
  };
  
  /**
   * 編集モードのキャンセル
   */
  const handleCancel = () => {
    setEditTitle(todo.title);  // 元のタイトルに戻す
    setIsEditing(false);
  };
  
  /**
   * キー入力イベントの処理（Enterキーで送信、Escキーでキャンセル）
   */
  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSubmit();
    } else if (e.key === 'Escape') {
      handleCancel();
    }
  };
  
  /**
   * 作成日時を相対時間で表示
   */
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return formatDistanceToNow(date, { addSuffix: true, locale: ja });
  };

  return (
    <Card className="mb-3 bg-white">
      <CardContent className="p-4 flex items-center justify-between">
        <div className="flex items-center gap-3 flex-1">
          {/* 完了状態のトグルチェックボックス */}
          <Checkbox 
            checked={todo.isCompleted} 
            onCheckedChange={() => onToggle(todo.id)}
            className="h-5 w-5"
          />
          
          {/* 編集モードか表示モードかで表示を切り替え */}
          {isEditing ? (
            <input
              type="text"
              value={editTitle}
              onChange={(e) => setEditTitle(e.target.value)}
              onBlur={handleSubmit}
              onKeyDown={handleKeyDown}
              className="flex-1 p-1 border rounded"
              autoFocus
            />
          ) : (
            <div className="flex flex-col flex-1">
              <span 
                className={todo.isCompleted ? "line-through text-gray-500" : ""}
                onDoubleClick={() => setIsEditing(true)}
              >
                {todo.title}
              </span>
              <span className="text-xs text-gray-500">{formatDate(todo.createdAt)}</span>
            </div>
          )}
        </div>
        
        <div className="flex gap-2">
          {/* 編集ボタン - 編集モードでなければ表示 */}
          {!isEditing && (
            <Button 
              variant="outline" 
              size="sm" 
              onClick={() => setIsEditing(true)}
            >
              編集
            </Button>
          )}
          
          {/* 削除ボタン */}
          <Button 
            variant="destructive" 
            size="sm" 
            onClick={() => onDelete(todo.id)}
          >
            削除
          </Button>
        </div>
      </CardContent>
    </Card>
  );
} 