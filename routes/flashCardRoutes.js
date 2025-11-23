import express from "express";
import {
  addFlashCard,
  fetchFlashCards,
  fetchFlashCardById,
  modifyFlashCard,
  removeFlashCard,
  findFlashCards,
} from "../controllers/flashCardController.js";

const router = express.Router();

/**
 * @swagger
 * components:
 *   schemas:
 *     FlashCard:
 *       type: object
 *       properties:
 *         id:
 *           type: string
 *           description: Auto-generated ID of the flash card
 *         created_at:
 *           type: date-time
 *           description: Time when flash card is generated
 *         question:
 *           type: string
 *           description: Question for this flash card
 *         answer:
 *           type: string
 *           description: Answer for this flash card
 *         times_missed:
 *           type: integer
 *           description: Number of times this flash card was answered wrong
 *         flashdeck_id:
 *           type: integer
 *           description: ID of the parent flash deck
 */

/**
 * @swagger
 * /api/flashcards:
 *   post:
 *     summary: Create a new flash card
 *     tags: [FlashCards]
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/FlashCard'
 *     responses:
 *       201:
 *         description: Flash card created successfully
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/FlashCard'
 *       400:
 *         description: Invalid input
 *   get:
 *     summary: Get all flash cards
 *     tags: [FlashCards]
 *     responses:
 *       200:
 *         description: List of all flash cards
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/FlashCard'
 *       500:
 *         description: Server error
 */
router.post("/", addFlashCard);
router.get("/", fetchFlashCards);

/**
 * @swagger
 * /api/flashcards/search:
 *   get:
 *     summary: Search for flash cards by their question
 *     tags: [FlashCards]
 *     parameters:
 *       - in: query
 *         name: query
 *         schema:
 *           type: string
 *         required: true
 *         description: Search term for flash card questions
 *     responses:
 *       200:
 *         description: List of matching flash cards
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/FlashCard'
 *       400:
 *         description: Missing search query
 *       500:
 *         description: Server error
 */
router.get("/search", findFlashCards);

/**
 * @swagger
 * /api/flashcards/{id}:
 *   get:
 *     summary: Get a flash card by ID
 *     tags: [FlashCards]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the flash card
 *     responses:
 *       200:
 *         description: Flash card details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/FlashCard'
 *       404:
 *         description: FlashCard not found
 *       500:
 *         description: Server error
 *   put:
 *     summary: Update a flash card
 *     tags: [FlashCards]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the flash card
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/FlashCard'
 *     responses:
 *       200:
 *         description: Updated flash card details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/FlashCard'
 *       404:
 *         description: Flash card not found
 *       400:
 *         description: Invalid input
 *   delete:
 *     summary: Delete a flash card
 *     tags: [FlashCards]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the flash card
 *     responses:
 *       200:
 *         description: Flash card deleted successfully
 *       500:
 *         description: Server error
 */
router.get("/:id", fetchFlashCardById);
router.put("/:id", modifyFlashCard);
router.delete("/:id", removeFlashCard);

export default router;