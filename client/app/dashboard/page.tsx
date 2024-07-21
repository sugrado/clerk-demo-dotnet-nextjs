"use client";
import { useAuth, UserButton } from "@clerk/nextjs";
import React from "react";

export default function Dashboard() {
  const { getToken } = useAuth();
  const handleSendRequest = async () => {
    const token = await getToken();

    const response = await fetch("http://localhost:5056/api/users/me", {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (response.ok) {
      alert("Request sent!");
    } else {
      alert("Failed to send request");
    }
  };
  return (
    <div>
      <UserButton />
      <h1>Dashboard</h1>
      <p>Welcome to your dashboard</p>
      <button onClick={handleSendRequest}>Send a request!</button>
    </div>
  );
}
