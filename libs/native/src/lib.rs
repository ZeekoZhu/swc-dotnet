#![recursion_limit = "2048"]
#![allow(dead_code)]

use std::{sync::Arc};

use interoptopus::{Inventory, InventoryBuilder, pattern};
use swc_core::{
    base::Compiler,
    common::{FilePathMapping, SourceMap},
};

mod parse;
mod util;

fn get_compiler() -> Arc<Compiler> {
    let cm = Arc::new(SourceMap::new(FilePathMapping::empty()));
    Arc::new(Compiler::new(cm))
}

pub fn my_inventory() -> Inventory {
    InventoryBuilder::new()
        .register(pattern!(parse::SwcWrap))
        .inventory()
}
