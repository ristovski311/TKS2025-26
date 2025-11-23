import express from "express";
import {
    addProfessor,
    fetchProfessors,
    fetchProfessorById,
    modifyProfessor,
    removeProfessor,
    findProfessors
} from "../controllers/professorController.js";

const router = express.Router();
/**
 * @swagger
 * components:
 *   schemas:
 *     Professor:
 *       type: object
 *       properties:
 *         id:
 *           type: string
 *           description: Auto-generated ID of the professor
 *         created_at:
 *           type: date-time
 *           description: Time when professor is generated
 *         first_name:
 *           type: string
 *           description: First name of the professor
 *         last_name:
 *           type: string
 *           description: Last name of the professor
 *         mail:
 *           type: string
 *           description: Professor's mail
 *         phone:
 *           type: string
 *           description: Professor's phone number
 *         office:
 *           type: string
 *           description: Professor's office
 */

/**
 * @swagger
 * /api/professors:
 *   post:
 *     summary: Create a new professor
 *     tags: [Professors]
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Professor'
 *     responses:
 *       201:
 *         description: Professor created successfully
 *         content: 
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Professor'
 *       400:
 *         description: Invalid input
 *     get:
 *       summary: Get all professors
 *       tags: [Professors]
 *       responses:
 *           200:
 *             description: List of all professors
 *             content:
 *               application/json:
 *                 schema:
 *                   type: array
 *                 items:
 *                   $ref: '#/components/schemas/Professor'
 *           500:
 *             description: Server error
 */
router.post("/professors", addProfessor);
router.get("/professors", fetchProfessors);

/**
 * @swagger
 * /api/professors/search:
 *   get:
 *     summary: Search for professors by username
 *     tags: [Professors]
 *     parameters:
 *       - in: query
 *         name: query
 *         schema:
 *           type: string
 *         required: true
 *         description: Search term for professors names
 *     responses:
 *       200:
 *         description: List of matching professors
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Professor'
 *       400:
 *         description: Missing search query
 *       500:
 *         description: Server error
 */
router.get("/professors/search", findProfessors);

/**
 * @swagger
 * /api/professors/{id}:
 *   get:
 *     summary: Get a professor by ID
 *     tags: [Professors]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the professor
 *     responses:
 *       200:
 *         description: Professor details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Professor'
 *       404:
 *         description: Professor not found
 *       500:
 *         description: Server error
 *   put:
 *     summary: Update a professor
 *     tags: [Professors]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the professor
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Professor'
 *     responses:
 *       200:
 *         description: Updated professor details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Professor'
 *       404:
 *         description: Professor not found
 *       400:
 *         description: Invalid input
 *   delete:
 *     summary: Delete a professor
 *     tags: [Professors]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the professor
 *     responses:
 *       200:
 *         description: Professor deleted successfully
 *       500:
 *         description: Server error
 */
router.get("/professors/:id", fetchProfessorById);
router.put("/professors/:id", modifyProfessor);
router.delete("/professors/:id", removeProfessor);

export default router;