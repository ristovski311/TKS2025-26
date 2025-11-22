import supabase from "../config/supabaseClient.js";

export const createFlashDeck = async (flashDeckData) => {
  const { data, error } = await supabase
    .from("FlashDeck")
    .insert([flashDeckData])
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const getFlashDecks = async () => {
  const { data, error } = await supabase.from("FlashDeck").select("*");

  if (error) throw new Error(error.message);
  return data;
};

export const getFlashDeckById = async (id) => {
  const { data, error } = await supabase
    .from("FlashDeck")
    .select("*")
    .eq("id", id)
    .single();

  if (error) throw new Error(error.message);
  return data;
};

export const updateFlashDeck = async (id, updates) => {
  const { data, error } = await supabase
    .from("FlashDeck")
    .update(updates)
    .eq("id", id)
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const deleteFlashDeck = async (id) => {
  const { error } = await supabase
    .from("FlashDeck")
    .delete()
    .eq("id", id);

  if (error) throw new Error(error.message);
  return { success: true, message: "Flash deck deleted successfully" };
};

export const searchFlashDecks = async (query) => {
  const { data, error } = await supabase
    .from("FlashDeck")
    .select("*")
    .ilike("title", `%${query}%`);

  if (error) throw new Error(error.message);
  return data;
};