import {
    createProfessor,
    getProfessors,
    getProfessorById,
    updateProfessor,
    deleteProfessor,
    searchProfessors
} from "../services/professorService.js"

export const addProfessor = async (req, res) => {
    try {
        const professorData = req.body;
        const newProfessor = await createProfessor(professorData);
        res.status(201).json(newProfessor);
    } catch (error) {
        res.status(400).json({ error: error.message });
    }
};

export const fetchProfessors = async (req, res) => {
    try {
        const professors = await getProfessors();
        res.status(200).json(professors);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};

export const fetchProfessorById = async (req, res) => {
    try {
        const { id } = req.params;
        const professor = await getProfessorById(id);
        if (!professor) {
            return res.status(404).json({ error: "Professor not found!" });
        }
        res.status(200).json(professor);
    } catch (error) {
        res.status(500).json({ error: error.message });
    };
};

export const modifyProfessor = async (req, res) => {
    try {
        const { id } = req.params;
        const updates = res.body;
        const updatedProfessor = await updateProfessor(id, updates);
        if (!updatedProfessor || updatedProfessor.length === 0) {
            return res.status(404).json({ error: "Professor not found!" });
        }
        res.status(200).json(updatedProfessor);
    } catch (error) {
        res.status(400).json({ error: error.message });
    }
};

export const removeProfessor = async (req, res) => {
    try {
        const { id } = req.params;
        const result = deleteProfessor(id);
        res.status(200).json(result);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};

export const findProfessors = async (req, res) => {
    try {
        const { query } = req.query;
        if (!query) {
            return res.status(400).json({ error: "Search query is required!" });
        }
        const professors = searchProfessors(query);
        res.status(200).json(professors);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};