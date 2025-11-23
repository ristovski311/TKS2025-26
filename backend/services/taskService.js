import supabase from "../config/supabaseClient.js"

export const createTask = async (taskData) => {
    const { data, error } = await supabase
        .from("Task")
        .insert([taskData])
        .select();

    if (error) throw new Error(error.message);
    return data;
};

export const getTasks = async () => {
    const { data, error } = await supabase
        .from("Task")
        .select("*");

    if (error) throw new Error(error.message);
    return data;
};

export const getTaskById = async (id) => {
    const { data, error } = await supabase
        .from("Task")
        .select("*")
        .eq("id", id)
        .single();

    if (error) throw new Error(error.message);
    return data;
};

export const updateTask = async (id, updates) => {
    const { data, error } = await supabase
        .from("Task")
        .update(updates)
        .eq("id", id)
        .select();

    if (error) throw new Error(error.message);
    return data;
};

export const deleteTask = async (id) => {
    const { data, error } = await supabase
        .from("Task")
        .delete()
        .eq("id", id);

    if (error) throw new Error(error.message);
    return { success: true, message: "Task deleted successfully" };
};

export const searchTasks = async (query) => {
    const { data, error } = await supabase
        .from("Task")
        .select("*")
        .ilike("title", `%${query}%`);

    if (error) throw new Error(error.message);
    return data;
}