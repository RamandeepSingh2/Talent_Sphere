1️⃣ Before writing any new code
Always update your local main branch:
 
## git checkout main
## git pull upstream main
 
 
 
2️⃣ Create a new feature branch
Never work directly on main.
 
## git checkout -b new-branch-name
 
 
 
3️⃣ Work on your code & test locally
Make changes
Test everything on your laptop
Make sure nothing breaks
 
 
 
4️⃣ Commit your changes
 
## git add .
## git commit -m "Meaningful message about what you changed"
 
 
 BEFORE raising PR → Now you pull again
This updates your branch with any changes others made.
Conflicts will happen here, but that's normal and expected.
After resolving, your branch will be clean and updated.
 
## git checkout main
## git pull upstream main
## git checkout new_branch-name
## git rebase main
 
 
 
5️⃣ Push your branch to your fork
 
## git push origin new-branch-name
 
 
 
6️⃣ Create a Pull Request (PR)
Open GitHub
Go to your fork
You will see a button: “Compare & Pull Request”
Choose upstream/main as target
Submit PR
 
 
7️⃣ Inform the team
“PR raised — Done.”
