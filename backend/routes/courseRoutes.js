import express from "express";
import {
  addCourse,
  fetchCourses,
  fetchCourseById,
  modifyCourse,
  removeCourse,
  findCourses,
} from "../controllers/courseController.js";

const router = express.Router();

/**
 * @swagger
 * components:
 *   schemas:
 *     Course:
 *       type: object
 *       properties:
 *         id:
 *           type: string
 *           description: Auto-generated ID of the course
 *         created_at:
 *           type: date-time
 *           description: Time when course is generated
 *         title:
 *           type: string
 *           description: Title of the course
 *         semester:
 *           type: integer
 *           description: Semester when course is being taught
 *         description:
 *           type: stirng
 *           description: Description of the course
 *         grade:
 *           type: integer
 *           description: Grade of the course
 *         user_id:
 *           type: integer
 *           description: ID of the user that created this subject
 *         professor_id:
 *           type: integer
 *           description: ID of the professor that teaches this subject
 */

/**
 * @swagger
 * /api/courses:
 *   post:
 *     summary: Create a new course
 *     tags: [Courses]
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Course'
 *     responses:
 *       201:
 *         description: Course created successfully
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Course'
 *       400:
 *         description: Invalid input
 *   get:
 *     summary: Get all courses
 *     tags: [Courses]
 *     responses:
 *       200:
 *         description: List of all courses
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Course'
 *       500:
 *         description: Server error
 */
router.post("/", addCourse);
router.get("/", fetchCourses);

/**
 * @swagger
 * /api/courses/search:
 *   get:
 *     summary: Search for courses by their title
 *     tags: [Courses]
 *     parameters:
 *       - in: query
 *         name: query
 *         schema:
 *           type: string
 *         required: true
 *         description: Search term for course titles
 *     responses:
 *       200:
 *         description: List of matching courses
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Course'
 *       400:
 *         description: Missing search query
 *       500:
 *         description: Server error
 */
router.get("/search", findCourses);

/**
 * @swagger
 * /api/courses/{id}:
 *   get:
 *     summary: Get a course by ID
 *     tags: [Courses]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the course
 *     responses:
 *       200:
 *         description: Course details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Course'
 *       404:
 *         description: Course not found
 *       500:
 *         description: Server error
 *   put:
 *     summary: Update a course
 *     tags: [Courses]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the course
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Course'
 *     responses:
 *       200:
 *         description: Updated course details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Course'
 *       404:
 *         description: Course not found
 *       400:
 *         description: Invalid input
 *   delete:
 *     summary: Delete a course
 *     tags: [Courses]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the course
 *     responses:
 *       200:
 *         description: Course deleted successfully
 *       500:
 *         description: Server error
 */
router.get("/:id", fetchCourseById);
router.put("/:id", modifyCourse);
router.delete("/:id", removeCourse);

export default router;