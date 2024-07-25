import { useAuth } from "@clerk/nextjs";
import axios from "axios";

const axiosInstance = axios.create({
  baseURL: "http://localhost:5056",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

export const handleSendRequest = async (token: string | null) => {
  const response = await axiosInstance.get("/api/users/me", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (response.status === 200) {
    alert("Request sent!");
  } else {
    alert("Failed to send request");
  }
};
