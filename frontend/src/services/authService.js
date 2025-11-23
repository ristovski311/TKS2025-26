import axios from "axios";

async function login(email, password) {
  try {
    const response = await axios.post("http://localhost:5000/api/auth/login", {
      headers: {
        "Content-Type": "application/json",
      },
      email: email,
      password: password,
    });
    return response.data;
  } catch (error) {
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Login failed");
    } else {
      throw new Error("Network error");
    }
  }
}

async function register(email, password, username, first_name, last_name) {
  try {
    await axios.post(
      "http://localhost:5000/api/auth/signup",
      {
        email: email,
        password: password,
      },
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    const response = await axios.post(
      "http://localhost:5000/api/users",
      {
        username: username,
        first_name: first_name,
        last_name: last_name,
      },
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    return response.data;
  } catch (error) {
    if (error.response && error.response.data) {
      throw new Error(
        error.response.data.message || "Creating an account failed"
      );
    } else {
      throw new Error("Network error");
    }
  }
}

export { login, register };
