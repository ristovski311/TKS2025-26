import { 
  createCourse, 
  getCourses, 
  getCourseById,
  updateCourse,
  deleteCourse,
  searchCourses
} from "../services/courseService.js";

export const addCourse = async (req, res) => {
  try {
    const courseData = req.body;
    const newCourse = await createCourse(courseData);
    res.status(201).json(newCourse);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const fetchCourses = async (req, res) => {
  try {
    const courses = await getCourses();
    res.status(200).json(courses);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const fetchCourseById = async (req, res) => {
  try {
    const { id } = req.params;
    const course = await getCourseById(id);
    if (!course) {
      return res.status(404).json({ error: "User not found" });
    }
    res.status(200).json(course);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const modifyCourse = async (req, res) => {
  try {
    const { id } = req.params;
    const updates = req.body;
    const updatedCourse = await updateCourse(id, updates);
    if (!updatedCourse || updateCourse.length === 0) {
      return res.status(404).json({ error: "User not found" });
    }
    res.status(200).json(updatedCourse);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const removeCourse = async (req, res) => {
  try {
    const { id } = req.params;
    const result = await deleteCourse(id);
    res.status(200).json(result);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const findCourses = async (req, res) => {
  try {
    const { query } = req.query;
    if (!query) {
      return res.status(400).json({ error: "Search query is required" });
    }
    const courses = await searchCourses(query);
    res.status(200).json(courses);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};