# Elesis Specialization Assets

Generated future-use sprites for Elesis specialization branches.

Confirmed class tree reference:

- Saber Knight -> Grand Master -> Empire Sword
- Pyro Knight -> Blazing Heart -> Flame Lord
- Dark Knight -> Crimson Avenger -> Bloody Queen
- Soar Knight -> Patrona -> Adrestia

Primary references:

- Official KOG Elesis character page: https://elsword.koggames.com/characters/elesis/
- Elsword Wiki Elesis class tree cross-check: https://elsword.fandom.com/wiki/Elesis

Asset convention:

- Store Elesis without specialization in `base/`.
- Store specialization art by path, then numbered evolution step:
  - `saber_knight_path/1_saber_knight/`
  - `saber_knight_path/2_grand_master/`
  - `saber_knight_path/3_empire_sword/`
  - `pyro_knight_path/1_pyro_knight/`
  - `pyro_knight_path/2_blazing_heart/`
  - `pyro_knight_path/3_flame_lord/`
  - `dark_knight_path/1_dark_knight/`
  - `dark_knight_path/2_crimson_avenger/`
  - `dark_knight_path/3_bloody_queen/`
  - `soar_knight_path/1_soar_knight/`
  - `soar_knight_path/2_patrona/`
  - `soar_knight_path/3_adrestia/`
- Runtime scenes should reference images through these path and step folders.
- `900x1000` PNG
- Transparent background
- Full-body character sprite
- Feet aligned near the lower edge
- Base, specialization, and evolution forms are wired into gameplay through the matching scene folders.
