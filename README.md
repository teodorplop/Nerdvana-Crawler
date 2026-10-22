# Nerdvana-Crawler

## Description

This app can be used to evaluate algorithmic tasks from different platforms and populate a Google Spreadsheet with the results.

### Supported platforms list:

* [Infoarena](https://infoarena.ro/)
* [Nerdarena](https://www.nerdarena.ro/)
* [Kilonova](https://kilonova.ro/)
* [Codeforces](https://codeforces.com/) (public problemset problems)

### Example

![image](https://github.com/teodorplop/Nerdvana-Crawler/assets/6988474/f13d768b-0496-437f-a382-60ca9b56f680)

## Usage

### Spreadsheet template

You can use this template for a quick start: [Nerdvana Crawler Template](https://docs.google.com/spreadsheets/d/1a1UavYkm0a-YGmpNm3JAvYMCHEUonKpNnOTp2qo2sNw/edit?usp=sharing).

### Spreadsheet rules

Your spreadsheet should follow a few rules. If you want to change the rules, you can do so by modifying [nerdvanaGoogleSheetConfig.json](https://github.com/teodorplop/Nerdvana-Crawler/blob/main/Resources/nerdvanaGoogleSheetConfig.json).

#### 1. Share spreadsheet

The spreadsheet should be shared with `nerdvanabot@nerdvanacrawler.iam.gserviceaccount.com` account, and permissions should be set to Editor:

![image](https://github.com/teodorplop/Nerdvana-Crawler/assets/6988474/8c7d286e-6a6f-450d-bb5e-83b2bca00416)

#### 2. Studenți

This subsheet needs to contain a mapping between the names of the students and their accounts on different platforms. These are the columns that can be populated:

![image](https://github.com/teodorplop/Nerdvana-Crawler/assets/6988474/4572d0ff-c40c-474c-aff0-7c2af388327d)

The value from the name column is the unique identifier of a student. 

#### 3. Cursul---

Any subsheet that starts with `Cursul` contains all students that participated in a course and the homeworks given to them. For example:

![image](https://github.com/teodorplop/Nerdvana-Crawler/assets/6988474/de56c103-4277-4d78-a68a-7cc1308a4039)

##### 3.a) Homework Group

In a course, you can create as many homework groups as you want. For example, you may have `Mandatory Group` and `Optional Group`. 

The homework group should be in the same column as the first task of the group! The recommendation is to merge the homework group name cells together.

##### 3.b) Task

You can add as many tasks as you want to a homework group. You can name your tasks however you please. 

The tasks needs to contain its URL, as you can see in the image above.

##### 3.c) Total

Each Homework Group needs to have a `Total` column, which will be populated automatically.

#### 4. Temă

Any subsheet that starts with `Temă` is used to aggregate results from all Homework Task Groups that exist in the spreadsheet. This is useful to see overall statistics of the homeworks.

For example, if you have a homework group named `Mandatory` which you used in multiple courses and you want to see the sum of all the scores your students achieved on that homework, you can create a subsheet called `Temă Mandatory`, and add your students in there:

![image](https://github.com/teodorplop/Nerdvana-Crawler/assets/6988474/dba71bc0-3ca6-4132-bc6d-153a3d73e09d)

## Running locally

### Requirements

* [.NET 9 SDK](https://dotnet.microsoft.com/download)
* A JSON key for the Google service account that the spreadsheets are shared with. Keep the key file outside the repository and never commit it.

### Credentials

The app reads the service account key from the `GOOGLE_CREDENTIALS` environment variable:

```
export GOOGLE_CREDENTIALS="$(cat ~/path/to/key.json)"
```

If the variable is not set, the app falls back to `Resources/credentials.json`. That file is ignored by git.

### Arguments

```
dotnet run --project NerdvanaCrawler.csproj -- -sheet <id-or-url> -homework last -taskgroup all -student all
```

| Argument | Value | If omitted |
|---|---|---|
| `-sheet` | Spreadsheet ID or full spreadsheet URL | Uses the `SHEET_ID` environment variable, otherwise asks in the console |
| `-homework` | Name of a `Cursul` subsheet, or `last` for the last one | Shows a menu |
| `-taskgroup` | Name of a homework group, or `all` | Shows a menu |
| `-student` | Student name, or `all` | Shows a menu |

## GitHub Actions

Each class has a workflow in `.github/workflows/` that evaluates its spreadsheet on a schedule and can also be run manually from the Actions tab. All class workflows call the shared workflow `homework_eval.yml`, which builds and runs the app.

### Secrets

Add these in the repository under Settings → Secrets and variables → Actions → Repository secrets:

| Secret | Value |
|---|---|
| `GOOGLE_CREDENTIALS` | Full contents of the service account JSON key |
| `SHEET_ID_<CLASS>` | Spreadsheet ID for one class, for example `SHEET_ID_CLASA10_2026_2027` |

Or with the [GitHub CLI](https://cli.github.com/):

```
gh secret set GOOGLE_CREDENTIALS < ~/path/to/key.json
gh secret set SHEET_ID_CLASA10_2026_2027
```

The spreadsheet ID is the part of the URL between `/d/` and `/edit`.

### Adding a workflow for a new class

1. Create the `SHEET_ID_<CLASS>` secret for the class's spreadsheet.
2. Copy one of the `weekly_runner_*.yml` files and give it a new name.
3. In the copy, change `name`, the `cron` schedule and the `SHEET_ID` secret it passes to `homework_eval.yml`.

Cron schedules use UTC. For example, `'00 12 * * 3'` runs every Wednesday at 12:00 UTC.

### Running manually

Open the class workflow in the Actions tab and click "Run workflow". Leave the sheet field empty to use the class's `SHEET_ID_<CLASS>` secret, or enter another spreadsheet ID to override it for that run.
