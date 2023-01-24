#![recursion_limit = "2048"]
#![allow(dead_code)]

use std::{sync::Arc};

use interoptopus::{Inventory, InventoryBuilder, pattern};
use swc_core::{
    base::Compiler,
    common::{sync::Lazy, FilePathMapping, SourceMap},
};

mod parse;
mod util;

static COMPILER: Lazy<Arc<Compiler>> = Lazy::new(|| {
    let cm = Arc::new(SourceMap::new(FilePathMapping::empty()));

    Arc::new(Compiler::new(cm))
});

fn get_compiler() -> Arc<Compiler> {
    COMPILER.clone()
}

pub fn my_inventory() -> Inventory {
    InventoryBuilder::new()
        .register(pattern!(parse::SwcWrap))
        .inventory()
}
