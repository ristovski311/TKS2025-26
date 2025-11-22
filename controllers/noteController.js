import { 
  createNote, 
  getNotes, 
  getNoteById,
  updateNote,
  deleteNote,
  searchNotes
} from "../services/noteService.js";

export const addNote = async (req, res) => {
  try {
    const noteData = req.body;
    const newNote = await createNote(noteData);
    res.status(201).json(newNote);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const fetchNotes = async (req, res) => {
  try {
    const notes = await getNotes();
    res.status(200).json(notes);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const fetchNoteById = async (req, res) => {
  try {
    const { id } = req.params;
    const note = await getNoteById(id);
    if (!note) {
      return res.status(404).json({ error: "Note not found" });
    }
    res.status(200).json(note);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const modifyNote = async (req, res) => {
  try {
    const { id } = req.params;
    const updates = req.body;
    const updatedNote = await updateNote(id, updates);
    if (!updatedNote || updatedNote.length === 0) {
      return res.status(404).json({ error: "Note not found" });
    }
    res.status(200).json(updatedNote);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

export const removeNote = async (req, res) => {
  try {
    const { id } = req.params;
    const result = await deleteNote(id);
    res.status(200).json(result);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

export const findNotes = async (req, res) => {
  try {
    const { query } = req.query;
    if (!query) {
      return res.status(400).json({ error: "Search query is required" });
    }
    const notes = await searchNotes(query);
    res.status(200).json(notes);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};