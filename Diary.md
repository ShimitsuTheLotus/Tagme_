# 2024
## February
### Feb. 13, 2024
- I've finished to add a motion to the program title when creating database.
- I've finished to solve the problem of global go back button. I also shared the template I made in my repo at: https://github.com/ShimizuTheLotus/GlobalBackButtonTemplate
- I've lost all my documents of Tagme_ ver 3 since a sync bug of OneNote.
- I tried to apply .resw files to globalization Tagme_.
### Feb. 14 2024
- I'm starting build page: CreateDataBase.
### Feb. 16 2024
- I finished reorder the layout when window size changed. Now it looks perfect.
### Feb. 21 2024
- I finished letting two buttons resizing to fit the window.
### Feb. 23 2024
- I find a way to solve the problem that after changing the color of TextBlock, the TextBlock can hardly resolve its color theme to the default: it could change its color automatically to fit the dark/light theme. The solution is: new a TextBlock and replace the original. However, it caused other problems, some codes setting the properties of the TextBlock will not make any difference. I found the problem and updated my UWP notes: https://github.com/ShimizuTheLotus/ShimizuTheLotus/blob/main/Note/UWP/CS%20And%20UI.md
### Feb. 26 2024
- I solved the reposition animation of the TextBlock when the text changed by putting it in a non child animation panel.
## March
### Mar. 4 2024
- I added some code to preview the style of database list.
### Mar. 6 2024
- I tried to run ver 2 build of Tagme_ and found it lost its certification.
### Mar. 8 2024
- Building the ListView in mainpage(NOT MainPage.xaml!)
## June
### Jun. 17 2024
- I finished the create database page and added some page transition motions.
### Jun. 29 2024
- I found my repositories are cloned by Gitcode, which belongs to CSDN, who stealed most of github open source repositories. After I knew that I made a **Special** update in README.md for them.

- **Anyone wanting to clone the repository should obey the GNU GPL 3.0, you shouldn't point the original author to anyother one, even that's a account with a same name with the author but on a different site, even that's a robot account. And, you should put the link of original author at a position easy to see, such as putting it below the modified version author's name.**
  
- **At least for this project, its only author is [ShimizuTheLotus](https://github.com/ShimizuTheLotus/) on github.com, and here's the [original project link](https://github.com/ShimizuTheLotus/Tagme_)**

## July
### Jul. 12 2024
- Created a new Page... To show the detail of a database. But now it's just a blank page.
### Jul. 24 2024
- found a way to add thousands separators in number (for int, use ".ToString("N0")". It will cause error when you use it to a string.)
### Jul. 29 2024
- Considering using WrapLayout and TokenizingTextBox in Windows Community Tookit Gallery. Fancy controls.
### Jul. 30 2024
- Failed to let tokens in TokenizingTextBox wrap. Discussion sended at https://github.com/CommunityToolkit/Windows/discussions/455 and wating for help.

## August
### Aug. 1 2024
- A misoperation let me uploaded compiled files and it made a change of thousands of lines.
- Then I found a **fatal error**: after I reinstalled it, the info database dosen't work. This means the problem of the database was solved when coding... I never tried to reinstall it.
### Aug. 2 2024
- The fatal error was solved.
### Aug. 7 2024
- Thanks for help by GitHub Support, a problem was solved.
- Filled https://github.com/CommunityToolkit with 6 issues. Too many bugs occur within TokenizingTextBox.
  - https://github.com/CommunityToolkit/Windows/issues/461
  - https://github.com/CommunityToolkit/Windows/issues/462
  - https://github.com/CommunityToolkit/Windows/issues/463
  - https://github.com/CommunityToolkit/Windows/issues/464
  - https://github.com/CommunityToolkit/Windows/issues/465
  - https://github.com/CommunityToolkit/Windows/issues/466
### Aug. 8 2024
- Due to stability concern, I'll remove control TokenizingTextBox in CommunityToolkit.Uwp. RichSuggestBox is considered as a succedaneum.
### Aug. 9 2024
- RichSuggestBox also have a bug(https://github.com/CommunityToolkit/Windows/issues/470), I'm concerned about its stability.
### Aug. 12 2024
- I don't know what happend, my local repository suddenly became strange and lost many changes haven't updated. At least I remember what I've changed... Maybe?
- Wow I haven't thought a repo not published and never be adverticed could be this popular!
- ![image](https://github.com/user-attachments/assets/df266bd6-d5ca-43be-b4e4-6cc90e29748c)
- This **ONE SO MANY PEOPLE** really loves my code a lot! But, this one a lot people, I don't like you.
- I've shared my ideas about RichSuggestBox in Community discussions,.
### Aug. 13 2024
- I found my ideas disappeared, maybe they're deleted. How dissatisfying. I'll consider removing RichSuggestBox from Tagme_, since it can't fit Tagme_.
- Tried to struggle in C++/CX to create custom control, I'm sure I hate it.
- When I trying to set x:Uid property for my self-made control, I found something interesting: After you set x:Uid, if there's a TextBlock in the control, it's text will be replaced with the target value.
### Aug. 14 2024
- I made the source code of the control a mess. I'll give writting function higher priority.
- I'm locked out by my Outlock account, I'm trying to get it back.
### Aug. 15 2024
- Thanks Junjie Zhu... You're always my **SAVIOUR**...
### Aug. 17 2024
- The build of new control is in progress. There are some bugs, though.
- AutoWrapPanel can wrap and align at top now.
![image](https://github.com/user-attachments/assets/36d30a12-ce16-4eb7-a9ca-30803ecea359)
### Aug. 19 2024
- Start making AutoWrapPanel can let children align at center. There are some bugs, though.
![image](https://github.com/user-attachments/assets/4dd4f42d-f9d6-4087-89e0-1f87c173b02f)
- Bug fixed, just perfect.
![image](https://github.com/user-attachments/assets/c206494d-1adb-496e-a3f3-c766c4620914)
### Aug. 20 2024
- AutoWrapPanel can now work properly when deleted item isat the first row.
- In the novel Journey to The West, Sun Wukong underwent 81 trials and tribulations. But while building Tagme_, I'll suffer 8,100 of that.
- **Things became strange after I added motion. The control won't work properly, so I removed that.**
