# Interpolation and extrapolation test bench / demo

This is a small test bench for testing interpolation of network transmitted movement with different interpolation solutions.

### Description
A sphere is pushed around randomly (deterministic according to a seed) and it's movement data is transmitted though a script that simulates common network transmission issues. This includes packet delay (with re-ordering) and packet loss. The movement data is then read and displayed by moving a sphere with a different color, and the network issued can be absolved by looking at the difference between the sphere objects. The result of a simple interpolation solution can be observed by selecting 'Linear' in the Type dropdown under 'Interpolation'.

## Description of all the available parameters:

1. Move Pattern
   - **Seed**: Seed that determents the basic pattern
   - **Stages**: How many pushes before the pattern starts over
   - **Max Force**: Max force of the push that moves the sphere
   - **Max Duration**: Max duration of the push that moves the sphere
   - **Max Speed**: Max speed of the sphere
   - **Send FPS**: How often movement data should be sent
2. Interpolation
   - **Type**: The type of interpolation method, only 'None' and 'Linear' available for now
   - **Client Delay**: How far back from the original gametime on the server, that the client should try to simulate. Higher means more margin for the client to get the delayed data, but lower means better responsiveness.
   - **History Length**: The sound of time the client should keep movement history, should be the max transmission delay and some more.
   - **Max Extrap.**: How long into the future the client is allowed to extrapolate/guess the position when data is missing. This is hard to do and really only matters when you get some serious packet loss
3. Delay
   - **Min**: Minimum base transmission delay
   - **Max**: Maximum base transmission delay
   - **Min Duration**: Minimum time before a new transmission delay is randomized from 'Min' and 'Max'
   - **Max Duration**: Maximum time before a new transmission delay is randomized from 'Min' and 'Max'
   - **Variation**: Constant random variation in transmission delay
4. Packet Loss
   - **Percent**: The chance of a total packet loss when packet loss is on
   - **Min Duration**: Minimum time for the sporadic packet loss
   - **Max Duration**: Maximum time for the sporadic packet loss
   - **Max Duration**: Minimum delay before another sporadic packet loss
   - **Max Duration**: Maximum delay before another sporadic packet loss
   - **Uniform Loss**: The constant chance for packet loss, always on
   
