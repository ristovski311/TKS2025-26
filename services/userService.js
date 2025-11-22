import supabase from "../config/supabaseClient.js";

export const createUser = async (userData) => {
  const { data, error } = await supabase
    .from("User")
    .insert([userData])
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const getUsers = async () => {
  const { data, error } = await supabase.from("User").select("*");

  if (error) throw new Error(error.message);
  return data;
};

export const getUserById = async (id) => {
  const { data, error } = await supabase
    .from("User")
    .select("*")
    .eq("id", id)
    .single();

  if (error) throw new Error(error.message);
  return data;
};

export const updateUser = async (id, updates) => {
  const { data, error } = await supabase
    .from("User")
    .update(updates)
    .eq("id", id)
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const deleteUser = async (id) => {
  const { error } = await supabase
    .from("User")
    .delete()
    .eq("id", id);

  if (error) throw new Error(error.message);
  return { success: true, message: "User deleted successfully" };
};

export const searchUsers = async (query) => {
  const { data, error } = await supabase
    .from("User")
    .select("*")
    .ilike("username", `%${query}%`);

  if (error) throw new Error(error.message);
  return data;
};