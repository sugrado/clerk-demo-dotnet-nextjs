"use client";
import {
  SignedIn,
  SignedOut,
  SignInButton,
  useAuth,
  UserButton,
} from "@clerk/nextjs";
import Dashboard from "./dashboard/page";
import { handleSendRequest } from "./utils/sampleRequest";

export default function Home() {
  const { getToken } = useAuth();

  return (
    <div>
      <SignedOut>
        <button onClick={async () => handleSendRequest(await getToken())}>
          Send a request!
        </button>
        <SignInButton />
      </SignedOut>
      <SignedIn>
        <Dashboard />
      </SignedIn>
    </div>
  );
}
