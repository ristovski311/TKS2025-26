import express from "express";
import {
    addTask,
    fetchTasks,
    fetchTaskById,
    modifyTask,
    removeTask,
    findTasks
} from "../controllers/taskController.js";

const router = express.Router();

/**
 * @swagger
 * components:
 *   schemas:
 *     Task:
 *       type: object
 *       properties:
 *         id:
 *           type: string
 *           description: Auto-generated ID of the task
 *         created_at:
 *           type: date-time
 *           description: Time when task is created
 *         title:
 *           type: string
 *           description: Task title
 *         type:
 *           type: string
 *           description: Type of the task
 *         description:
 *           type: string
 *           description: Task's description
 *         date:
 *           type: date-time
 *           description: When is task scheduled for
 *         completed:
 *           type: boolean
 *           description: Whether the task is completed or not
 *         grade_max:
 *           type: number
 *           description: Maximum grade one can earn for given task
 *         grade_earned:
 *           type: number
 *           description: Grade one has earned for given task
 *         course_id:
 *           type: integer
 *           description: ID of the taks's matching course
 */

/**
 * @swagger
 * /api/tasks:
 *   post:
 *     summary: Create a new task
 *     tags: [Tasks]
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Task'
 *     responses:
 *       201:
 *         description: Task created successfully
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Task'
 *       400:
 *         description: Invalid input
 *   get:
 *     summary: Get all tasks
 *     tags: [Tasks]
 *     responses:
 *       200:
 *         description: List of all tasks
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Task'
 *       500:
 *         description: Server error
 */
router.post("/tasks", addTask);
router.get("/tasks", fetchTasks);

/**
 * @swagger
 * /api/tasks/search:
 *   get:
 *     summary: Search for tasks by their title
 *     tags: [Tasks]
 *     parameters:
 *       - in: query
 *         name: query
 *         schema:
 *           type: string
 *         required: true
 *         description: Search term for tasks
 *     responses:
 *       200:
 *         description: List of matching tasks
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Task'
 *       400:
 *         description: Missing search query
 *       500:
 *         description: Server error
 */
router.get("/tasks/search", findTasks);

/**
 * @swagger
 * /api/tasks/{id}:
 *   get:
 *     summary: Get a task by ID
 *     tags: [Tasks]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the task
 *     responses:
 *       200:
 *         description: Task details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Task'
 *       404:
 *         description: Task not found
 *       500:
 *         description: Server error
 *   put:
 *     summary: Update a task
 *     tags: [Tasks]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the task
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Task'
 *     responses:
 *       200:
 *         description: Updated task details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Task'
 *       404:
 *         description: Task not found
 *       400:
 *         description: Invalid input
 *   delete:
 *     summary: Delete a task
 *     tags: [Tasks]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the task
 *     responses:
 *       200:
 *         description: Task deleted successfully
 *       500:
 *         description: Server error
 */
router.get("/tasks/:id", fetchTaskById);
router.put("/tasks/:id", modifyTask);
router.delete("/tasks/:id", removeTask);

export default router;