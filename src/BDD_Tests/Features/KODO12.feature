﻿@Justin
Feature: Ability to see a Steam Library after signing in

As a user, I want the be able to go to the library page and see my Steam games present on the page.
If there is no Steam account linked, then I should be presented with a message telling me what I need
	to do in order to make the page load what's expected.

@LibraryWithNoSteam
Scenario: Library page will show a message when a user doesn't have a linked Steam account
	Given I am a visitor
	When I click on the library link
	Then The page shows me a message
