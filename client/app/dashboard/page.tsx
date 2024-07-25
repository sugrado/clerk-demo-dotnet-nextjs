"use client";
import { useAuth, UserButton } from "@clerk/nextjs";
import React from "react";
import { handleSendRequest } from "../utils/sampleRequest";

export default function Dashboard() {
  const { getToken } = useAuth();

  return (
    <div>
      <UserButton />
      <h1>Dashboard</h1>
      <p>Welcome to your dashboard</p>
      <button onClick={async () => handleSendRequest(await getToken())}>
        Send a request!
      </button>
    </div>
  );
}
