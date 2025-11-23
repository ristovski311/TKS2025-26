import { 
  createFlashCard, 
  getFlashCards, 
  getFlashCardById,
  updateFlashCard,
  deleteFlashCard,
  searchFlashCards
} from "../services/flashCardService.js";

export const addFlashCard = async (req, res) => {
  try {
    const flashCardData = req.body;
    const newFlashCard = await createFlashCard(flashCardData);
    res.status(201).json(newFlashCard);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const fetchFlashCards = async (req, res) => {
  try {
    const flashCards = await getFlashCards();
    res.status(200).json(flashCards);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const fetchFlashCardById = async (req, res) => {
  try {
    const { id } = req.params;
    const flashCard = await getFlashCardById(id);
    if (!flashCard) {
      return res.status(404).json({ error: "Flash card not found" });
    }
    res.status(200).json(flashCard);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const modifyFlashCard = async (req, res) => {
  try {
    const { id } = req.params;
    const updates = req.body;
    const updatedFlashCard = await updateFlashCard(id, updates);
    if (!updatedFlashCard || updatedFlashCard.length === 0) {
      return res.status(404).json({ error: "Flash card not found" });
    }
    res.status(200).json(updatedFlashCard);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const removeFlashCard = async (req, res) => {
  try {
    const { id } = req.params;
    const result = await deleteFlashCard(id);
    res.status(200).json(result);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const findFlashCards = async (req, res) => {
  try {
    const { query } = req.query;
    if (!query) {
      return res.status(400).json({ error: "Search query is required" });
    }
    const flashCards = await searchFlashCards(query);
    res.status(200).json(flashCards);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};