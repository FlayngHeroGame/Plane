public void Release() {
    if (pullLength < minPull) {
        return; // Avoid launching if the pull is too weak
    }
    launched = true; // Only set launched to true if the pull is sufficient
    ApplyLaunchForce();
}