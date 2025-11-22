import supabase from "../config/supabaseClient.js";

export const createFlashCard = async (flashCardData) => {
  const { data, error } = await supabase
    .from("FlashCard")
    .insert([flashCardData])
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const getFlashCards = async () => {
  const { data, error } = await supabase.from("FlashCard").select("*");

  if (error) throw new Error(error.message);
  return data;
};

export const getFlashCardById = async (id) => {
  const { data, error } = await supabase
    .from("FlashCard")
    .select("*")
    .eq("id", id)
    .single();

  if (error) throw new Error(error.message);
  return data;
};

export const updateFlashCard = async (id, updates) => {
  const { data, error } = await supabase
    .from("FlashCard")
    .update(updates)
    .eq("id", id)
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const deleteFlashCard = async (id) => {
  const { error } = await supabase
    .from("FlashCard")
    .delete()
    .eq("id", id);

  if (error) throw new Error(error.message);
  return { success: true, message: "Flash card deleted successfully" };
};

export const searchFlashCards = async (query) => {
  const { data, error } = await supabase
    .from("FlashCard")
    .select("*")
    .ilike("question", `%${query}%`);

  if (error) throw new Error(error.message);
  return data;
};