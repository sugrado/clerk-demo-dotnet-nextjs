import { SignedIn, SignedOut, SignInButton, UserButton } from "@clerk/nextjs";
import Dashboard from "./dashboard/page";

export default async function Home() {
  return (
    <div>
      <SignedOut>
        <SignInButton />
      </SignedOut>
      <SignedIn>
        <Dashboard />
      </SignedIn>
    </div>
  );
}
