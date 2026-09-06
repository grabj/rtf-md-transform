# Konwersja plików **RTF → Markdown**.

## Polecenie

**rtf-md-transform.exe <wejście> [wyjście]**

## Argumenty

| Argument  | Wymagany | Opis                                        |
|-----------|:--------:|---------------------------------------------|
| `wejście` |  TAK     | Plik `.rtf` albo katalog z plikami `.rtf`   |
| `wyjście` |  NIE     | Plik `.md` albo katalog wynikowy            |

## Zachowanie

| `wejście` | `wyjście`  | Polecenie                                                | Rezultat                                                        |
|-----------|-----------|----------------------------------------------------------|-----------------------------------------------------------------|
| plik      | —         | `rtf-md-transform.exe note.rtf`                          | `note.rtf` → `note.md` (ten sam katalog)                        |
| katalog   | —         | `rtf-md-transform.exe ".\notes"`                         | każdy `.rtf` → `.md` obok siebie (z podkatalogami)              |
| plik      | plik      | `rtf-md-transform.exe note.rtf wynik.md`                 | `note.rtf` → `wynik.md`                                         |
| plik      | katalog   | `rtf-md-transform.exe note.rtf ".\md\"`                  | `note.rtf` → `.\md\` → `.\md\note.md`                           |
| katalog   | katalog   | `rtf-md-transform.exe ".\notes" ".\notes-md"`            | `.\notes` → `.\notes-md` (struktura zachowana)                  |

- Podkatalogi i struktura folderów są zachowane.
- Pliki `.rtf` nie są usuwane.
