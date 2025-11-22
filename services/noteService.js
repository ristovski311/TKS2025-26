import supabase from "../config/supabaseClient.js";

export const createNote = async (noteData) => {
  const { data, error } = await supabase
    .from("Note")
    .insert([noteData])
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const getNotes = async () => {
  const { data, error } = await supabase.from("Note").select("*");

  if (error) throw new Error(error.message);
  return data;
};

export const getNoteById = async (id) => {
  const { data, error } = await supabase
    .from("Note")
    .select("*")
    .eq("id", id)
    .single();

  if (error) throw new Error(error.message);
  return data;
};

export const updateNote = async (id, updates) => {
  const { data, error } = await supabase
    .from("Note")
    .update(updates)
    .eq("id", id)
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const deleteNote = async (id) => {
  const { error } = await supabase
    .from("Note")
    .delete()
    .eq("id", id);

  if (error) throw new Error(error.message);
  return { success: true, message: "Note deleted successfully" };
};

export const searchNotes = async (query) => {
  const { data, error } = await supabase
    .from("Note")
    .select("*")
    .ilike("title", `%${query}%`);

  if (error) throw new Error(error.message);
  return data;
};