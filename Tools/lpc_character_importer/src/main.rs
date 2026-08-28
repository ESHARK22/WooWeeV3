use image::{RgbaImage, imageops};
use rand::RngExt;
use rand::seq::IndexedRandom;
use serde::{Deserialize, Serialize};
use std::fs::{self, File};
use std::io::Write;
use std::path::{Path, PathBuf};

const DEFAULT_ASSET_DIR: &str = "./assets";
const OUTPUT_DIR: &str = "./output";

#[derive(Serialize, Deserialize, Debug, Clone, PartialEq)]
pub struct Item {
    pub name: String,
    pub color: String,
}

#[derive(Serialize, Deserialize, Debug, Clone, PartialEq)]
pub struct Character {
    pub id: String,
    pub gender: String,
    pub expression: String,
    pub hair: Option<Item>,
    pub shirt: Option<Item>,
    pub pants: Option<Item>,
    pub shoes: Option<Item>,
    pub hat: Option<Item>,
}

impl Character {
    pub fn is_female(&self) -> bool {
        self.gender == "female"
    }

    fn find_nested_idle(start_dir: &Path, gender: &str) -> Option<PathBuf> {
        if !start_dir.exists() {
            return None;
        }

        let gender_target = start_dir.join(gender).join("idle.png");
        if gender_target.exists() {
            return Some(gender_target);
        }

        if gender == "female" {
            let thin_target = start_dir.join("thin").join("idle.png");
            if thin_target.exists() {
                return Some(thin_target);
            }
        }

        let unisex_target = start_dir.join("idle.png");
        if unisex_target.exists() {
            return Some(unisex_target);
        }

        if let Ok(entries) = fs::read_dir(start_dir) {
            for entry in entries.flatten() {
                let path = entry.path();
                if path.is_dir() {
                    let sub_gender = path.join(gender).join("idle.png");
                    if sub_gender.exists() {
                        return Some(sub_gender);
                    }
                    if gender == "female" {
                        let sub_thin = path.join("thin").join("idle.png");
                        if sub_thin.exists() {
                            return Some(sub_thin);
                        }
                    }
                    let sub_idle = path.join("idle.png");
                    if sub_idle.exists() {
                        return Some(sub_idle);
                    }
                }
            }
        }
        None
    }

    /// Resolves file paths and assigns the requested color to each layer
    pub fn get_layer_paths(&self, asset_dir: &str) -> Vec<(PathBuf, String)> {
        let mut layers = Vec::new();
        let base = PathBuf::from(asset_dir);
        let g = if self.is_female() { "female" } else { "male" };
        let g_head = if self.is_female() {
            "female_small"
        } else {
            "male_small"
        };

        // Shadow (Untinted)
        Self::push_layer(&mut layers, &[base.join("shadow/adult/idle.png")], "None");

        // Base Body (Untinted to preserve skin tone)
        Self::push_layer(
            &mut layers,
            &[base.join(format!("body/bodies/{}/idle.png", g))],
            "None",
        );

        // Pants
        if let Some(pants) = &self.pants {
            if let Some(p) = Self::find_nested_idle(&base.join("legs").join(&pants.name), g) {
                layers.push((p, pants.color.clone()));
            }
        }

        // Shoes
        if let Some(shoes) = &self.shoes {
            if let Some(p) = Self::find_nested_idle(&base.join("feet").join(&shoes.name), g) {
                layers.push((p, shoes.color.clone()));
            }
        }

        // Shirt
        if let Some(shirt) = &self.shirt {
            if let Some(p) =
                Self::find_nested_idle(&base.join("torso/clothes").join(&shirt.name), g)
            {
                layers.push((p, shirt.color.clone()));
            } else if let Some(p) =
                Self::find_nested_idle(&base.join("torso/aprons").join(&shirt.name), g)
            {
                layers.push((p, shirt.color.clone()));
            }
        }

        // Head Base & Face (Untinted)
        Self::push_layer(
            &mut layers,
            &[
                base.join(format!("head/heads/human/{}/idle.png", g_head)),
                base.join(format!("head/heads/human/{}/idle.png", g)),
            ],
            "None",
        );

        Self::push_layer(
            &mut layers,
            &[
                base.join(format!("head/faces/{}/{}/idle.png", g, self.expression)),
                base.join(format!("head/faces/global/{}/idle.png", self.expression)),
            ],
            "None",
        );

        // Hair
        if let Some(hair) = &self.hair {
            if let Some(p) = Self::find_nested_idle(&base.join("hair").join(&hair.name), g) {
                layers.push((p, hair.color.clone()));
            }
        }

        // Hat
        if let Some(hat) = &self.hat {
            if let Some(p) = Self::find_nested_idle(&base.join("hat/cloth").join(&hat.name), g) {
                layers.push((p, hat.color.clone()));
            } else if let Some(p) =
                Self::find_nested_idle(&base.join("hat/magic").join(&hat.name), g)
            {
                layers.push((p, hat.color.clone()));
            } else if let Some(p) =
                Self::find_nested_idle(&base.join("hat/formal").join(&hat.name), g)
            {
                layers.push((p, hat.color.clone()));
            }
        }

        layers
    }

    fn push_layer(layers: &mut Vec<(PathBuf, String)>, candidates: &[PathBuf], color: &str) {
        for path in candidates {
            if path.exists() {
                layers.push((path.clone(), color.to_string()));
                return;
            }
        }
    }

    pub fn random(id: String) -> Self {
        let mut rng = rand::rng();

        let genders = ["male", "female"];
        let expressions = ["neutral", "happy", "anger", "sad", "blush", "shock"];
        let colors = [
            "Red", "Blue", "Green", "Black", "Brown", "Yellow", "Purple", "Orange", "Navy", "Pink",
        ];

        let hair_options = [
            Some("flat_top_fade"),
            Some("bob"),
            Some("curtains"),
            Some("messy1"),
            Some("long"),
        ];
        let shirt_options = ["shortsleeve", "longsleeve", "overalls"];
        let pants_options = ["pants", "pantaloons", "hose", "leggings"];
        let shoes_options = ["boots/rimmed", "boots/basic", "shoes/basic", "slippers"];
        let hat_options = ["bandana", "hood", "leather_cap", "tophat", "wizard"];

        let gender = *genders.choose(&mut rng).unwrap();

        Self {
            id,
            gender: gender.to_string(),
            expression: expressions.choose(&mut rng).unwrap().to_string(),
            hair: hair_options.choose(&mut rng).unwrap().map(|name| Item {
                name: name.to_string(),
                color: colors.choose(&mut rng).unwrap().to_string(),
            }),
            shirt: Some(Item {
                name: shirt_options.choose(&mut rng).unwrap().to_string(),
                color: colors.choose(&mut rng).unwrap().to_string(),
            }),
            pants: Some(Item {
                name: pants_options.choose(&mut rng).unwrap().to_string(),
                color: colors.choose(&mut rng).unwrap().to_string(),
            }),
            shoes: Some(Item {
                name: shoes_options.choose(&mut rng).unwrap().to_string(),
                color: colors.choose(&mut rng).unwrap().to_string(),
            }),
            hat: if rng.random_bool(0.40) {
                Some(Item {
                    name: hat_options.choose(&mut rng).unwrap().to_string(),
                    color: colors.choose(&mut rng).unwrap().to_string(),
                })
            } else {
                None
            },
        }
    }
}

fn get_rgb(color: &str) -> [u8; 3] {
    match color {
        "Red" => [210, 40, 40],
        "Blue" => [40, 100, 220],
        "Navy" => [20, 40, 100],
        "Green" => [40, 180, 60],
        "Black" => [40, 40, 45],
        "White" => [255, 255, 255],
        "Brown" => [120, 70, 30],
        "Yellow" => [240, 210, 40],
        "Purple" => [140, 50, 190],
        "Orange" => [230, 130, 30],
        "Pink" => [240, 100, 150],
        _ => [255, 255, 255],
    }
}

/// Takes a base image and tints it. Uses Luminosity blending to preserve highlights/shadows.
fn apply_tint(img: &mut RgbaImage, color_name: &str) {
    if color_name == "None" {
        return;
    }

    let [cr, cg, cb] = get_rgb(color_name);

    for pixel in img.pixels_mut() {
        let a = pixel.0[3];
        if a > 0 {
            let r = pixel.0[0] as f32;
            let g = pixel.0[1] as f32;
            let b = pixel.0[2] as f32;

            // Calculate the grayscale luminosity of the pixel
            let mut luma = (0.299 * r + 0.587 * g + 0.114 * b) / 255.0;

            // Multiply luminosity by our target color
            pixel.0[0] = (luma * cr as f32).clamp(0.0, 255.0) as u8;
            pixel.0[1] = (luma * cg as f32).clamp(0.0, 255.0) as u8;
            pixel.0[2] = (luma * cb as f32).clamp(0.0, 255.0) as u8;

            // pixel.0[0] = (cr as f32).clamp(0.0, 255.0) as u8;
            // pixel.0[1] = (cg as f32).clamp(0.0, 255.0) as u8;
            // pixel.0[2] = (cb as f32).clamp(0.0, 255.0) as u8;
        }
    }
}

/// Composites all present layers onto one canvas, applying color tints
fn composite_layers(
    layers: &[(PathBuf, String)],
) -> Result<Option<RgbaImage>, Box<dyn std::error::Error>> {
    let mut composite: Option<RgbaImage> = None;

    for (path, color) in layers {
        if path.exists() {
            let mut layer = image::open(path)?.to_rgba8();

            // Apply the color tint to this specific layer before overlaying
            apply_tint(&mut layer, color);

            if let Some(ref mut base) = composite {
                imageops::overlay(base, &layer, 0, 0);
            } else {
                composite = Some(layer);
            }
        }
    }

    Ok(composite)
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let characters_count = 200;
    let output_sprites_dir = Path::new(OUTPUT_DIR).join("sprites");
    let output_data_dir = Path::new(OUTPUT_DIR).join("data");

    fs::create_dir_all(&output_sprites_dir)?;
    fs::create_dir_all(&output_data_dir)?;

    let mut database: Vec<Character> = Vec::new();

    println!(
        "Generating {} beautifully colored characters...",
        characters_count
    );

    for i in 1..=characters_count {
        let char_id = format!("char_{:03}", i);
        let character = Character::random(char_id.clone());

        // Get layers and their chosen colors
        let layers = character.get_layer_paths(DEFAULT_ASSET_DIR);

        // Composite and tint
        if let Some(composite) = composite_layers(&layers)? {
            let sprite_path = output_sprites_dir.join(format!("{}.png", char_id));
            composite.save(&sprite_path)?;
        }

        // Save
        let json_path = output_data_dir.join(format!("{}.json", char_id));
        let json_file = File::create(json_path)?;
        serde_json::to_writer_pretty(json_file, &character)?;

        database.push(character);
    }

    // Save Master
    let master_db_path = output_data_dir.join("characters.json");
    let mut master_file = File::create(master_db_path)?;
    let serialized_db = serde_json::to_string_pretty(&database)?;
    master_file.write_all(serialized_db.as_bytes())?;

    println!("Done! Check output/sprites/ and output/data/");
    Ok(())
}
