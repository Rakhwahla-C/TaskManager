import { useState, useEffect } from "react";
import { format } from "date-fns";
import { AnimatePresence, motion } from "motion/react";
export default function TaskManager() {
  const [taskDescription, setTaskDescription] = useState("");
  const [tasks, setTasks] = useState([]);
  const [currentTime, setCurrentTime] = useState(new Date());
  const [editingTask, setEditingTask] = useState(null);
  const [completedTasks, setCompletedTasks] = useState({});
  const [editedDescription, setEditedDescription] = useState("");

  const API_URL = import.meta.env.VITE_API_URL;

  useEffect(() => {
    fetchTasks();
  }, []);

  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(interval);
  }, []);
  const fetchTasks = async () => {
    try {
      const response = await fetch(API_URL);
      if (!response.ok) throw new Error("Failed to fetch");
      const data = await response.json();
      setTasks(data);
    } catch (err) {
      console.error("Fetch error:", err);
    }
  };

  const taskDone = (id) => {
    setCompletedTasks((prev) => ({ ...prev, [id]: true }));

    DeleteTask(id);
    setCompletedTasks((prev) => {
      const copy = { ...prev };
      delete copy[id];
      return copy;
    });
  };

  const addTask = async () => {
    if (!taskDescription.trim()) return;

    const payload = { description: taskDescription };

    try {
      const response = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const error = await response.text();
        throw new Error(`Failed to add: ${error}`);
      }

      const newTaskFromServer = await response.json();
      setTasks((prev) => [...prev, newTaskFromServer]);
      setTaskDescription("");
    } catch (err) {
      console.error("Add error:", err);
    }
  };

  const DeleteTask = async (id) => {
    try {
      const response = await fetch(`${API_URL}/${id}`, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      });

      if (!response.ok) {
        const error = await response.text();
        throw new Error(`Failed to delete: ${error}`);
      }

      setTasks((prev) => prev.filter((task) => task.TaskId !== id));
    } catch (err) {
      console.error("Delete error:", err);
    }
  };

  const UpdateTask = async (id, newDescription) => {
    try {
      const response = await fetch(`${API_URL}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ description: newDescription }),
      });

      if (!response.ok) {
        const error = await response.text();
        throw new Error(`Failed to update: ${error}`);
      }
      const updated = await response.json();
      setTasks((prev) =>
        prev.map((task) =>
          task.TaskId === id ? { ...task, Description: newDescription } : task
        )
      );
    } catch (err) {
      console.error("Update error:", err);
    }
  };

  const formattedDate = format(new Date(), "EEE, dd MMM yyyy");
  const formattedTime = format(currentTime, "hh:mm:ss a");

  return (
    <div className="flex justify-center bg-black min-h-screen  text-white">
      <div className="mt-10 mb-10 w-full lg:w-1/2 px-4">
        <div className="flex justify-between items-center mb-6">
          <p className="">{formattedDate}</p>
          <p>{formattedTime}</p>
        </div>

        <div className="flex border border-green-800 rounded-lg rounded-r-lg ">
          <input
            type="text"
            placeholder="Enter task..."
            value={taskDescription}
            onChange={(e) => setTaskDescription(e.target.value)}
            onKeyUp={(e) => e.key === "Enter" && addTask()}
            className="bg-transparent p-3 flex-1 focus:ring focus:ring-green-500 focus:outline-none rounded-l-lg"
          />
          <button
            onClick={addTask}
            className="bg-green-600 hover:bg-green-400 px-6 rounded-r-md  font-semibold  active:bg-green-800 hover:cursor-pointer"
          >
            Add
          </button>
        </div>

        <div className="mt-6 space-y-3">
          {tasks.length === 0 ? (
            <p className="text-gray-500 text-center py-8">
              No tasks yet. Add one!
            </p>
          ) : (
            <AnimatePresence>
              {tasks.map((task) => (
                <motion.div
                  key={task.TaskId}
                  className={`flex justify-between items-center border border-gray-600 rounded-lg p-4 hover:bg-gray-900 `}
                  whileHover={{ scale: 1.03 }}
                  transition={{ ease: "easeInOut", duration: 0.5 }}
                  initial={{ opacity: 0, scale: 0 }}
                  animate={{ opacity: 1, scale: 1 }}
                  exit={{ opacity: 0, scale: 0 }}
                >
                  <p>{task.Description}</p>
                  <div className={`flex gap-2 `}>
                    <motion.button
                      onClick={() => taskDone(task.TaskId)}
                      className={`border border-green-500 hover:bg-green-500 px-4 py-1 rounded hover:cursor-pointer active:bg-green-700 
                  `}
                      whileTap={{ scale: 0.9 }}
                    >
                      <span className="hidden md:block ">Done</span>
                      <span className="block md:hidden">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke-width="1.5"
                          stroke="currentColor"
                          class="size-5"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            d="m4.5 12.75 6 6 9-13.5"
                          />
                        </svg>
                      </span>
                    </motion.button>

                    <motion.button
                      className="border border-amber-500 hover:bg-amber-500 font-semibold px-4 py-0.5 rounded active:bg-amber-700"
                      onClick={() => {
                        setEditingTask(task);
                        setEditedDescription(task.Description);
                      }}
                      whileTap={{ scale: 0.9 }}
                    >
                      <span className="hidden md:block">Edit</span>
                      <span className="block md:hidden">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke-width="1.5"
                          stroke="currentColor"
                          className="size-5"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10"
                          />
                        </svg>
                      </span>
                    </motion.button>

                    <motion.button
                      className="border border-red-500 hover:bg-red-500 px-4 py-1 rounded hover:cursor-pointer active:bg-red-700"
                      onClick={() => DeleteTask(task.TaskId)}
                      whileTap={{ scale: 0.9 }}
                    >
                      <span className="hidden md:block">Delete</span>
                      <span className="block md:hidden ">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke-width="1.5"
                          stroke="currentColor"
                          className="size-5"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0"
                          />
                        </svg>
                      </span>
                    </motion.button>
                  </div>
                </motion.div>
              ))}
            </AnimatePresence>
          )}
        </div>
      </div>
      {editingTask && (
        <div className="fixed inset-0 bg-black bg-opacity-60 flex items-center justify-center z-50">
          <div className="bg-gray-800 text-white p-6 rounded-lg w-80 shadow-lg">
            <h2 className="text-lg font-semibold mb-3">Edit Task</h2>

            <input
              className="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:outline-none focus:ring focus:ring-amber-500"
              value={editedDescription}
              onChange={(e) => setEditedDescription(e.target.value)}
              onKeyUp={(e) =>
                e.key === "Enter" &&
                UpdateTask(editingTask.TaskId, editedDescription) &&
                setEditingTask(null)
              }
            />

            <div className="flex justify-end gap-2 mt-4">
              <motion.button
                className="px-4 py-1 bg-gray-600 rounded hover:bg-gray-500"
                onClick={() => setEditingTask(null)}
                whileTap={{ scale: 0.9 }}
              >
                Cancel
              </motion.button>

              <motion.button
                className="px-4 py-1 bg-amber-600 rounded hover:bg-amber-500"
                onClick={() => {
                  UpdateTask(editingTask.TaskId, editedDescription);
                  setEditingTask(null);
                }}
                whileTap={{ scale: 0.9 }}
              >
                Save
              </motion.button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
