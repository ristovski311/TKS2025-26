import { 
  createFlashDeck, 
  getFlashDecks, 
  getFlashDeckById,
  updateFlashDeck,
  deleteFlashDeck,
  searchFlashDecks
} from "../services/flashDeckService.js";

export const addFlashDeck = async (req, res) => {
  try {
    const flashDeckData = req.body;
    const newFlashDeck = await createFlashDeck(flashDeckData);
    res.status(201).json(newFlashDeck);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const fetchFlashDecks = async (req, res) => {
  try {
    const flashDecks = await getFlashDecks();
    res.status(200).json(flashDecks);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const fetchFlashDeckById = async (req, res) => {
  try {
    const { id } = req.params;
    const flashDeck = await getFlashDeckById(id);
    if (!flashDeck) {
      return res.status(404).json({ error: "Flash deck not found" });
    }
    res.status(200).json(flashDeck);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const modifyFlashDeck = async (req, res) => {
  try {
    const { id } = req.params;
    const updates = req.body;
    const updatedFlashDeck = await updateFlashDeck(id, updates);
    if (!updatedFlashDeck || updatedFlashDeck.length === 0) {
      return res.status(404).json({ error: "Flash deck not found" });
    }
    res.status(200).json(updatedFlashDeck);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const removeFlashDeck = async (req, res) => {
  try {
    const { id } = req.params;
    const result = await deleteFlashDeck(id);
    res.status(200).json(result);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const findFlashDecks = async (req, res) => {
  try {
    const { query } = req.query;
    if (!query) {
      return res.status(400).json({ error: "Search query is required" });
    }
    const flashDecks = await searchFlashDecks(query);
    res.status(200).json(flashDecks);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};