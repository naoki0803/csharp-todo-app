import { useState, useEffect } from "react";
import { Todo, todoApi } from "@/lib/api-client";
import { toast } from "sonner";

export const useTodos = () => {
    const [todos, setTodos] = useState<Todo[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetchTodos();
    }, []);

    const fetchTodos = async () => {
        try {
            setLoading(true);
            const data = await todoApi.getAll();
            setTodos(data);
            setError(null);
        } catch (err) {
            console.error("Todoの取得に失敗しました", err);
            setError("Todoの取得に失敗しました。");
            toast.error("Todoの取得に失敗しました。");
        } finally {
            setLoading(false);
        }
    };

    const addTodo = async (title: string) => {
        try {
            const newTodo = await todoApi.create({ title });
            setTodos((prevTodos) => [newTodo, ...prevTodos]);
            toast.success("Todoを追加しました");
        } catch (err) {
            console.error("Todoの追加に失敗しました", err);
            toast.error("Todoの追加に失敗しました");
        }
    };

    const deleteTodo = async (id: string) => {
        try {
            await todoApi.delete(id);
            setTodos((prevTodos) => prevTodos.filter((todo) => todo.id !== id));
            toast.success("Todoを削除しました");
        } catch (err) {
            console.error("Todoの削除に失敗しました", err);
            toast.error("Todoの削除に失敗しました");
        }
    };

    const toggleTodo = async (id: string) => {
        try {
            await todoApi.toggleCompletion(id);
            setTodos((prevTodos) =>
                prevTodos.map((todo) =>
                    todo.id === id
                        ? { ...todo, isCompleted: !todo.isCompleted }
                        : todo
                )
            );

            const toggledTodo = todos.find((todo) => todo.id === id);
            const status =
                toggledTodo && !toggledTodo.isCompleted ? "完了" : "未完了";
            toast.success(`Todoを${status}にしました`);
        } catch (err) {
            console.error("Todoの状態変更に失敗しました", err);
            toast.error("Todoの状態変更に失敗しました");
        }
    };

    const updateTodo = async (id: string, title: string) => {
        try {
            await todoApi.update(id, { title });
            setTodos((prevTodos) =>
                prevTodos.map((todo) =>
                    todo.id === id ? { ...todo, title } : todo
                )
            );
            toast.success("Todoを更新しました");
        } catch (err) {
            console.error("Todoの更新に失敗しました", err);
            toast.error("Todoの更新に失敗しました");
        }
    };

    return {
        todos,
        loading,
        error,
        addTodo,
        deleteTodo,
        toggleTodo,
        updateTodo,
    };
};
