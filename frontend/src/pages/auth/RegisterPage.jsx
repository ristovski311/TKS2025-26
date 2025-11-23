import { useState } from "react";
import { register } from "../../services/authService.js";
import { useNavigate } from "react-router";

function RegisterPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [username, setUserName] = useState("");
  const [first_name, setFirstName] = useState("");
  const [last_name, setLastName] = useState("");
  const [error, setError] = useState("");

  let navigate = useNavigate();

  const handleAccountCreation = async () => {
    setError("");
    try {
      const user = await register(
        email,
        password,
        username,
        first_name,
        last_name
      );

      console.log("Account creation successfull!", user);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <main className="w-screen h-screen flex flex-col items-center justify-center px-4 bg-white">
      <div className="max-w-sm w-full text-gray-600 space-y-5">
        <div className="text-center pb-8">
          <div className="mt-5">
            <h3 className="text-gray-800 text-2xl font-bold sm:text-3xl">
              Create an account
            </h3>
          </div>
        </div>
        <form onSubmit={(e) => e.preventDefault()} className="space-y-5">
          <div>
            <label className="font-medium">Email</label>
            <input
              type="email"
              required
              onChange={(e) => setEmail(e.target.value)}
              className="w-full mt-2 px-3 py-2 text-gray-500 bg-transparent outline-none border focus:border-indigo-600 shadow-sm rounded-lg"
            />
          </div>
          <div>
            <label className="font-medium">Password</label>
            <input
              type="password"
              required
              onChange={(e) => setPassword(e.target.value)}
              className="w-full mt-2 px-3 py-2 text-gray-500 bg-transparent outline-none border focus:border-indigo-600 shadow-sm rounded-lg"
            />
          </div>
          <div>
            <label className="font-medium">Username</label>
            <input
              type="username"
              required
              onChange={(e) => setUserName(e.target.value)}
              className="w-full mt-2 px-3 py-2 text-gray-500 bg-transparent outline-none border focus:border-indigo-600 shadow-sm rounded-lg"
            />
          </div>
          <div>
            <label className="font-medium">First name</label>
            <input
              type="first_name"
              required
              onChange={(e) => setFirstName(e.target.value)}
              className="w-full mt-2 px-3 py-2 text-gray-500 bg-transparent outline-none border focus:border-indigo-600 shadow-sm rounded-lg"
            />
          </div>
          <div>
            <label className="font-medium">Last name</label>
            <input
              type="last_name"
              required
              onChange={(e) => setLastName(e.target.value)}
              className="w-full mt-2 px-3 py-2 text-gray-500 bg-transparent outline-none border focus:border-indigo-600 shadow-sm rounded-lg"
            />
          </div>

          {error && <p className="text-red-500 text-sm">{error}</p>}

          <button
            onClick={handleAccountCreation}
            className="w-full px-4 py-2 text-white font-medium bg-indigo-600 hover:bg-indigo-500 active:bg-indigo-600 rounded-lg duration-150"
          >
            Sign in
          </button>
        </form>
        <p className="text-center">
          Already have an account?{" "}
          <a
            onClick={() => navigate(-1)}
            className="font-medium text-indigo-600 hover:text-indigo-500"
          >
            Log in
          </a>
        </p>
      </div>
    </main>
  );
}

export default RegisterPage;
