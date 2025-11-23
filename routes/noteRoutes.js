import express from "express";
import {
  addNote,
  fetchNotes,
  fetchNoteById,
  modifyNote,
  removeNote,
  findNotes,
} from "../controllers/noteController.js";

const router = express.Router();

/**
 * @swagger
 * components:
 *   schemas:
 *     Note:
 *       type: object
 *       properties:
 *         id:
 *           type: string
 *           description: Auto-generated ID of the note
 *         created_at:
 *           type: date-time
 *           description: Time when note is generated
 *         title:
 *           type: string
 *           description: Title of the note
 *         description:
 *           type: string
 *           description: Description of the note
 *         content:
 *           type: string
 *           description: Content of the note
 *         last_updated:
 *           type: date-time
 *           description: Time when note was last updated
 *         type:
 *           type: string
 *           description: Type of this note (note or folder)
 *         course_id:
 *           type: integer
 *           description: ID of the course this note belongs to
 *         parent_id:
 *           type: integer
 *           description: ID of the parent of this note (or folder)
 */

/**
 * @swagger
 * /api/notes:
 *   post:
 *     summary: Create a new note
 *     tags: [Notes]
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Note'
 *     responses:
 *       201:
 *         description: Note created successfully
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Note'
 *       400:
 *         description: Invalid input
 *   get:
 *     summary: Get all notes
 *     tags: [Notes]
 *     responses:
 *       200:
 *         description: List of all notes
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Note'
 *       500:
 *         description: Server error
 */
router.post("/", addNote);
router.get("/", fetchNotes);

/**
 * @swagger
 * /api/notes/search:
 *   get:
 *     summary: Search for notes by their title
 *     tags: [Notes]
 *     parameters:
 *       - in: query
 *         name: query
 *         schema:
 *           type: string
 *         required: true
 *         description: Search term for note titles
 *     responses:
 *       200:
 *         description: List of matching notes
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Note'
 *       400:
 *         description: Missing search query
 *       500:
 *         description: Server error
 */
router.get("/search", findNotes);

/**
 * @swagger
 * /api/notes/{id}:
 *   get:
 *     summary: Get a note by ID
 *     tags: [Notes]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the note
 *     responses:
 *       200:
 *         description: Note details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Note'
 *       404:
 *         description: Note not found
 *       500:
 *         description: Server error
 *   put:
 *     summary: Update a note
 *     tags: [Notes]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the note
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/Note'
 *     responses:
 *       200:
 *         description: Updated note details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Note'
 *       404:
 *         description: Note not found
 *       400:
 *         description: Invalid input
 *   delete:
 *     summary: Delete a note
 *     tags: [Notes]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the note
 *     responses:
 *       200:
 *         description: Note deleted successfully
 *       500:
 *         description: Server error
 */
router.get("/:id", fetchNoteById);
router.put("/:id", modifyNote);
router.delete("/:id", removeNote);

export default router;