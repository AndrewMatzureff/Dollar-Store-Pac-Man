# Dollar-Store-Pac-Man
This repo houses my second Unity game project (since beginning my latest attempt at learning Unity in 2025) which is intended to serve as a basic Pac-Man knockoff.

## Repo Setup Template
Use this section as a template to set up repos for subsequent Unity projects with Git LFS for efficient storage of assets in addition to exclusion of game build files.

1. Clone repo into new project root from parent directory and navigate to project root.

- `git clone git@github.com:<username>/<repo>.git && cd <repo>/`

2. Add graphics and sound assets to large file storage.

- `git lfs track "*.mp3" "*.png"`

3. Open `.gitignore` file and append the assembled artifact exclusions to the end of the file.

```

# === Custom === #

# Assembled Artifacts
/*_[Dd]ata/
/*.exe
/*.zip

```

4. Stage, commit and push file tracking updates.

- `git add .gitattributes .gitignore && git commit -m "Track .MP3 and .PNG assets in Git LFS & ignore assembled artifacts" && git push`
