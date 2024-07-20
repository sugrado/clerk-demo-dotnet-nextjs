import { SignedIn, SignedOut, SignInButton, UserButton } from "@clerk/nextjs";

export default async function Home() {
  return (
    <div>
      <SignedOut>
        <SignInButton />
      </SignedOut>
      <SignedIn>
        <UserButton />
      </SignedIn>
    </div>
  );
}
