# **Open World Developer Repo**

## **Introduction**

This is the developer only version of the open world mod and allows all of our developers to work from one space. Create branches as you like however try not to unless nessesary as sometimes merging them can be problematic depending on the amount of unresolved conflicts. For any questions regarding the repo either message D12 or Primate as they should be able to guide you or fix any problems you are having.

## **How to commit changes**

To commit changes to this repo you either need git terminal or git desktop installed on your system, I would personally recommend desktop as terminal became much more strict as of recent so you would need to use a key and ssh for terminal. On the other hand, to use github desktop all you have to do is go here https://desktop.github.com/ and download the version that is corrasponding with your os. To download this from linux you'll have to find a semi long tutorial where you do it through commands in terminal. Once you have you git source control installed from there you will have to link the repositry, to do this you will need to clone the repo onto your local machine via the github desktop app or through setting up a folder for the project and running the following commands in terminal "git init" -> git clone INSERTREPOLINK. After this you should have the code on your local system and be able to easily upload your changes to the github.

After you have made a change to the code there is two ways you can commit your changes to the repo. Either, do it through terminal which you will have to run the following commands 'git add -A && git commit -a -m "My commit message"' this will add all of the changed files to a new commit called "My Commit message". Alternitively, you can use the easier way of using github desktop where you first open the repo in the app and make sure you can see your changes on the right hand side and then simply committing the changes by typing a commit message and pressing the commit button.

## **How to push your changes**

To push your changes to the repo (update the repo with your code) you must first have all of the changes you want to push committed. Next, assuming you have everything commited and your repo set up, through terminal you have to run the command "git push -u origin BRANCHNAME" the branch name here would be the branch you would like to commit the code to such as the "main" branch. Through github desktop, you can do this very easily in comparision simply by pressing the big "push origin" button in the middle of the screen after committing your changes.

## **How to create and use a new branch**

To create a new branch the easiest ways are either through the website, where you can simply click on branches and add a new one or by using github desktop and by pressing on switch branch and creating a new one based off a stable version of the mod. After the creation of a new branch make sure the rest of the team knows as to not leave them in the dark so we do not step on each others feet and mess up each others code. To start making changes to your new branch on github desktop simply switch to the newly created branch or in terminal simply push your changes to the new branch name. when creating branches try to keep them named in a way that other people can easily understand as to not cause too much confusion when it comes to branches.

