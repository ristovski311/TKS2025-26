import supabase from "../config/supabaseClient.js";

export const createCourse = async (courseData) => {
  const { data, error } = await supabase
    .from("Course")
    .insert([courseData])
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const getCourses = async () => {
  const { data, error } = await supabase.from("Course").select("*");

  if (error) throw new Error(error.message);
  return data;
};

export const getCourseById = async (id) => {
  const { data, error } = await supabase
    .from("Course")
    .select("*")
    .eq("id", id)
    .single();

  if (error) throw new Error(error.message);
  return data;
};

export const updateCourse = async (id, updates) => {
  const { data, error } = await supabase
    .from("Course")
    .update(updates)
    .eq("id", id)
    .select();

  if (error) throw new Error(error.message);
  return data;
};

export const deleteCourse = async (id) => {
  const { error } = await supabase
    .from("Course")
    .delete()
    .eq("id", id);

  if (error) throw new Error(error.message);
  return { success: true, message: "Course deleted successfully" };
};

export const searchCourses = async (query) => {
  const { data, error } = await supabase
    .from("Course")
    .select("*")
    .ilike("title", `%${query}%`);

  if (error) throw new Error(error.message);
  return data;
};