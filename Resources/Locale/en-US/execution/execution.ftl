execution-verb-name = Execute
execution-verb-message = Use your weapon to execute someone.

# All the below localisation strings have access to the following variables
# attacker (the person committing the execution)
# victim (the person being executed)
# weapon (the weapon used for the execution)

execution-popup-melee-initial-internal = You ready {THE($weapon)} against {THE($victim)}'s throat.
execution-popup-melee-initial-external = { CAPITALIZE(THE($attacker)) } readies {POSS-ADJ($attacker)} {$weapon} against the throat of {THE($victim)}.
execution-popup-melee-complete-internal = You slit the throat of {THE($victim)}!
execution-popup-melee-complete-external = { CAPITALIZE(THE($attacker)) } slits the throat of {THE($victim)}!

execution-popup-self-initial-internal = You ready {THE($weapon)} against your own throat.
execution-popup-self-initial-external = { CAPITALIZE(THE($attacker)) } readies {POSS-ADJ($attacker)} {$weapon} against their own throat.
execution-popup-self-complete-internal = You slit your own throat!
execution-popup-self-complete-external = { CAPITALIZE(THE($attacker)) } slits their own throat!

# Gun execution
execution-popup-gun-initial-internal = You ready the muzzle of {THE($weapon)} against {THE($victim)}'s head.
execution-popup-gun-initial-external = { CAPITALIZE(THE($attacker)) } readies the muzzle of {THE($weapon)} against the head of {THE($victim)}.
execution-popup-gun-complete-internal = You blast {THE($victim)} in the head!
execution-popup-gun-complete-external = { CAPITALIZE(THE($attacker)) } blasts {THE($victim)} in the head!
execution-popup-gun-empty = {CAPITALIZE(THE($weapon))} clicks.

# Gun suicide
suicide-popup-gun-initial-internal = You place the muzzle of {THE($weapon)} in your mouth.
suicide-popup-gun-initial-external = { CAPITALIZE(THE($attacker)) } places the muzzle of {THE($weapon)} in {POSS-ADJ($attacker)} mouth.
suicide-popup-gun-complete-internal = You shoot yourself in the head!
suicide-popup-gun-complete-external = { CAPITALIZE(THE($attacker)) } shoots {REFLEXIVE($attacker)} in the head!
