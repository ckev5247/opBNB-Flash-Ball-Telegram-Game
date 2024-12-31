// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

contract FlashBallGame {
    // Mapping to track ball points (true/false) for each player
    mapping(address => bool) private playerBallStatus;

    // Events
    event BallStatusUpdated(address indexed player, bool status);
    event BallStatusReset(address indexed player);

    // Function to set a player's ball status to true
    function activateBallStatus(address player) external {
        require(player != address(0), "Invalid player address");

        playerBallStatus[player] = true;

        emit BallStatusUpdated(player, true);
    }

    // Function to reset a player's ball status to false
    function resetBallStatus(address player) external {
        require(player != address(0), "Invalid player address");

        playerBallStatus[player] = false;

        emit BallStatusReset(player);
    }

    // Function to view a player's current ball status
    function getPlayerBallStatus(address player) external view returns (bool) {
        return playerBallStatus[player];
    }
}
