import { useState } from "react";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";
import "./index.css";
import TaskManager from "./components/TaskManager";

function App() {
  const [count, setCount] = useState(0);

  return (
    <>
      <TaskManager />
    </>
  );
}

export default App;
