import supabase from "../config/supabaseClient.js"

export const createProfessor = async (professorData) => {
    const { data, error } = await supabase
        .from("Professor")
        .insert([professorData])
        .select();

    if (error) throw new Error(error.message);
    return data;
};

export const getProfessors = async () => {
    const { data, error } = await supabase
        .from("Professor")
        .select("*");

    if (error) throw new Error(error.message);
    return data;
};

export const getProfessorById = async (id) => {
    const { data, error } = await supabase
        .from("Professor")
        .select("*")
        .eq("id", id)
        .single();

    if (error) throw new Error(error.message);
    return data;
};

export const updateProfessor = async (id, updates) => {
    const { data, error } = await supabase
        .from("Professor")
        .update(updates)
        .eq("id", id)
        .select();

    if (error) throw new Error(error.message);
    return data;
};

export const deleteProfessor = async (id) => {
    const { data, error } = await supabase
        .from("Professor")
        .delete()
        .eq("id", id);

    if (error) throw new Error(error.message);
    return { success: true, message: "Professor deleted successfully" };
};

export const searchProfessors = async (query) => {
    const { data, error } = await supabase
        .from("Professor")
        .select("*")
        .ilike("title", `%${query}%`);

    if (error) throw new Error(error.message);
    return data;
}