import express from "express";
import cors from "cors";
import dotenv from "dotenv";
import userRoutes from "./routes/userRoutes.js";
import taskRoutes from "./routes/taskRoutes.js";
import professorRoutes from "./routes/professorRoutes.js";
import courseRoutes from "./routes/courseRoutes.js";
import noteRoutes from "./routes/noteRoutes.js";
import flashCardRoutes from "./routes/flashCardRoutes.js";
import flashDeckRoutes from "./routes/flashDeckRoutes.js"
import { swaggerUi, specs } from "./config/swagger.js";

dotenv.config();

const app = express();
app.use(cors());
app.use(express.json());

// Swagger documentation route
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(specs));


app.use("/api/users", userRoutes);
app.use("/api/tasks", taskRoutes);
app.use("/api/professors", professorRoutes);
app.use("/api/notes", noteRoutes);
app.use("/api/flashCards", flashCardRoutes);
app.use("/api/flashDecks", flashDeckRoutes);
app.use("/api/courses", courseRoutes);

app.get("/", (req, res) => {
  res.send("API is running... <a href='/api-docs'>View API documentation</a>");
});

const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
  console.log(`Server running on http://localhost:${PORT}`);
  console.log(`API documentation available at http://localhost:${PORT}/api-docs`);
});
