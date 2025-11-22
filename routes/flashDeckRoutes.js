import express from "express";
import {
  addFlashDeck,
  fetchFlashDecks,
  fetchFlashDeckById,
  modifyFlashDeck,
  removeFlashDeck,
  findFlashDecks,
} from "../controllers/flashDeckController.js";

const router = express.Router();

/**
 * @swagger
 * components:
 *   schemas:
 *     FlashDeck:
 *       type: object
 *       properties:
 *         id:
 *           type: string
 *           description: Auto-generated ID of the flash deck
 *         created_at:
 *           type: date-time
 *           description: Time when flash deck is generated
 *         title:
 *           type: string
 *           description: Title of this flash deck
 *         description:
 *           type: string
 *           description: Description of this flash deck
 *         times_studied:
 *           type: integer
 *           description: Number of times this flash deck was studied
 *         score:
 *           type: number
 *           description: ID of the parent flash deck
 *         best_score:
 *           type: number
 *           description: High score of user for this flash deck
 *         course_id:
 *           type: integer
 *           description: ID of the course this flash deck is made for
 */

/**
 * @swagger
 * /api/flashdecks:
 *   post:
 *     summary: Create a new flash deck
 *     tags: [FlashDecks]
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/FlashDeck'
 *     responses:
 *       201:
 *         description: Flash deck created successfully
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/FlashDeck'
 *       400:
 *         description: Invalid input
 *   get:
 *     summary: Get all flash decks
 *     tags: [FlashDecks]
 *     responses:
 *       200:
 *         description: List of all flash decks
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/FlashDeck'
 *       500:
 *         description: Server error
 */
router.post("/flashdecks", addFlashDeck);
router.get("/flashdecks", fetchFlashDecks);

/**
 * @swagger
 * /api/flashdecks/search:
 *   get:
 *     summary: Search for flash decks by their title
 *     tags: [FlashDecks]
 *     parameters:
 *       - in: query
 *         name: query
 *         schema:
 *           type: string
 *         required: true
 *         description: Search term for flash deck titles
 *     responses:
 *       200:
 *         description: List of matching flash decks
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/FlashDeck'
 *       400:
 *         description: Missing search query
 *       500:
 *         description: Server error
 */
router.get("/flashdecks/search", findFlashDecks);

/**
 * @swagger
 * /api/flashdecks/{id}:
 *   get:
 *     summary: Get a flash deck by ID
 *     tags: [FlashDecks]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the flash deck
 *     responses:
 *       200:
 *         description: Flash deck details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/FlashDeck'
 *       404:
 *         description: FlashDeck not found
 *       500:
 *         description: Server error
 *   put:
 *     summary: Update a flash deck
 *     tags: [FlashDecks]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the flash deck
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/FlashDeck'
 *     responses:
 *       200:
 *         description: Updated flash deck details
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/FlashDeck'
 *       404:
 *         description: Flash deck not found
 *       400:
 *         description: Invalid input
 *   delete:
 *     summary: Delete a flash deck
 *     tags: [FlashDecks]
 *     parameters:
 *       - in: path
 *         name: id
 *         schema:
 *           type: string
 *         required: true
 *         description: ID of the flash deck
 *     responses:
 *       200:
 *         description: Flash deck deleted successfully
 *       500:
 *         description: Server error
 */
router.get("/flashdecks/:id", fetchFlashDeckById);
router.put("/flashdecks/:id", modifyFlashDeck);
router.delete("/flashdecks/:id", removeFlashDeck);

export default router;