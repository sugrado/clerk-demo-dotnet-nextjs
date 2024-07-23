"use client";
import { SignedIn, SignedOut, SignInButton, UserButton } from "@clerk/nextjs";
import Dashboard from "./dashboard/page";
import { handleSendRequest } from "./utils/sampleRequest";

export default function Home() {
  const handleButton = async () => {
    await handleSendRequest();
  };
  return (
    <div>
      <SignedOut>
        <button onClick={handleButton}>Send a request!</button>
        <SignInButton />
      </SignedOut>
      <SignedIn>
        <Dashboard />
      </SignedIn>
    </div>
  );
}
