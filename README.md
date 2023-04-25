# **Open World Developer Repo**

## **Introduction**

This is the developer-only version of the Open World mod and allows all of our developers to work from one space. Create branches as you like; however, try not to merge them unless necessary, as sometimes merging them can be problematic depending on the amount of unresolved conflicts. For any questions regarding the repo, either message D12 or Primate, as they should be able to guide you or fix any problems you are having.

## **How to commit changes**

To commit changes to this repo, you either need git terminal or git desktop installed on your system. I personally recommend desktop, as terminal has become much more strict in recent years, so you would need to use a key and ssh for terminal. On the other hand, to use Github Desktop, all you have to do is go here: https://desktop.github.com/ and download the version that is compatible with your OS. To download this from Linux, you'll have to find a semi-long tutorial where you do it through commands in the terminal. Once you have your git source control installed, you will have to link the repository. To do this, you will need to clone the repo onto your local machine via the github desktop app or by setting up a folder for the project and running the following commands in the terminal: "git init" -> "git clone INSERTREPOLINK". After this, you should have the code on your local system and be able to easily upload your changes to GitHub.

After you have made a change to the code, there are two ways you can commit your changes to the repo. Either do it through the terminal, where you will have to run the following commands: 'git add -A && git commit -a -m "My commit message"'. This will add all of the changed files to a new commit called "My Commit message". Alternitively, you can use the easier way of using github desktop, where you first open the repo in the app and make sure you can see your changes on the right-hand side, and then simply commit the changes by typing a commit message and pressing the commit button.

## **How to push your changes**

To push your changes to the repo (update the repo with your code), you must first have all of the changes you want to push committed. Next, assuming you have everything committed and your repo set up, through the terminal, you have to run the command "git push -u origin BRANCHNAME" The branch name here would be the branch you would like to commit the code to, such as the "main" branch. Through github desktop, you can do this very easily in comparison simply by pressing the big "push origin" button in the middle of the screen after committing your changes.

## **How to create and use a new branch**

To create a new branch, the easiest way is either through the website, where you can simply click on branches and add a new one, or by using github desktop and pressing on switch branch and creating a new one based off a stable version of the mod. After the creation of a new branch, make sure the rest of the team knows so we do not leave them in the dark and mess up each other's code. To start making changes to your new branch on github desktop, simply switch to the newly created branch, or in terminal, simply push your changes to the new branch name. When creating branches, try to keep them named in a way that other people can easily understand so as not to cause too much confusion when it comes to branches.
